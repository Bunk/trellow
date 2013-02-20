﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using Microsoft.Phone.Shell;
using TrelloNet;
using trello.Services;
using trello.Services.Handlers;

namespace trello.ViewModels
{
    public sealed class CardDetailOverviewViewModel : PivotItemViewModel,
                                                      IConfigureTheAppBar,
                                                      IHandle<CardDescriptionChanged>,
                                                      IHandle<CardDueDateChanged>,
                                                      IHandle<CardLabelAdded>,
                                                      IHandle<CardLabelRemoved>,
                                                      IHandle<CardDetailChecklistViewModel.ChecklistAggregationsUpdated>,
                                                      IHandle<CardDetailMembersViewModel.MemberAggregationsUpdated>
    {
        private readonly ITrello _api;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProgressService _progress;
        private readonly IWindowManager _windowManager;
        private int _attachments;
        private int _checkItems;
        private int _checkItemsChecked;
        private int _checklists;
        private string _desc;
        private DateTime? _due;
        private int _members;
        private string _name;
        private int _votes;

        public string Id { get; private set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;

                NotifyOfPropertyChange(() => Name);
            }
        }

        public string Desc
        {
            get { return _desc; }
            set
            {
                if (value == _desc) return;
                _desc = value;
                NotifyOfPropertyChange(() => Desc);
            }
        }

        public DateTime? Due
        {
            get { return _due; }
            set
            {
                if (value.Equals(_due)) return;
                _due = value;
                NotifyOfPropertyChange(() => Due);
            }
        }

        public int Checklists
        {
            get { return _checklists; }
            set
            {
                if (value == _checklists) return;
                _checklists = value;
                NotifyOfPropertyChange(() => Checklists);
            }
        }

        public int CheckItems
        {
            get { return _checkItems; }
            set
            {
                if (value == _checkItems) return;
                _checkItems = value;
                NotifyOfPropertyChange(() => CheckItems);
            }
        }

        public int CheckItemsChecked
        {
            get { return _checkItemsChecked; }
            set
            {
                if (value == _checkItemsChecked) return;
                _checkItemsChecked = value;
                NotifyOfPropertyChange(() => CheckItemsChecked);
            }
        }

        public int Votes
        {
            get { return _votes; }
            set
            {
                if (value == _votes) return;
                _votes = value;
                NotifyOfPropertyChange(() => Votes);
            }
        }

        public int Attachments
        {
            get { return _attachments; }
            set
            {
                if (value == _attachments) return;
                _attachments = value;
                NotifyOfPropertyChange(() => Attachments);
            }
        }

        public int Members
        {
            get { return _members; }
            set
            {
                if (value == _members) return;
                _members = value;
                NotifyOfPropertyChange(() => Members);
            }
        }

        public string CoverUri { get; set; }

        public int CoverHeight { get; set; }

        public int CoverWidth { get; set; }

        public IObservableCollection<LabelViewModel> Labels { get; set; }

        public CardDetailOverviewViewModel(ITrello api,
                                           IProgressService progress,
                                           IEventAggregator eventAggregator,
                                           IWindowManager windowManager)
        {
            DisplayName = "overview";

            _api = api;
            _progress = progress;
            _eventAggregator = eventAggregator;
            _windowManager = windowManager;
            _eventAggregator.Subscribe(this);

            Labels = new BindableCollection<LabelViewModel>();
        }

        public CardDetailOverviewViewModel Initialize(Card card)
        {
            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;
            Due = card.Due;
            Checklists = card.Checklists.Count;
            CheckItems = card.Checklists.Aggregate(0, (i, model) => i + model.CheckItems.Count);
            CheckItemsChecked = card.Checklists.Aggregate(0,
                                                          (i, model) => i + model.CheckItems.Count(item => item.Checked));
            Votes = card.Badges.Votes;
            Attachments = card.Attachments.Count;
            Members = card.Members.Count;

            var cover = card.Attachments.SingleOrDefault(att => att.Id == card.IdAttachmentCover);
            CoverUri = cover != null ? cover.Previews.First().Url : null;
            CoverHeight = cover != null ? cover.Previews.First().Height : 0;
            CoverWidth = cover != null ? cover.Previews.First().Width : 0;

            var lbls = card.Labels.Select(lbl => new LabelViewModel(lbl.Color.ToString(), lbl.Name));
            Labels.Clear();
            Labels.AddRange(lbls);

            return this;
        }

        public ApplicationBar Configure(ApplicationBar existing)
        {
            existing.Buttons.Add(new AppBarButton
            {
                Text = "delete card",
                IconUri = new Uri("/Assets/Icons/dark/appbar.delete.rest.png", UriKind.Relative),
                Message = "DeleteCard"
            });
            return existing;
        }

        public void Handle(CardDescriptionChanged message)
        {
            Desc = message.Description;
        }

        public void Handle(CardDueDateChanged message)
        {
            Due = message.DueDate;
        }

        public void Handle(CardLabelAdded message)
        {
            Labels.Add(new LabelViewModel(message.Color.ToString(), message.Name));
        }

        public void Handle(CardLabelRemoved message)
        {
            LabelViewModel found = Labels.FirstOrDefault(lbl => lbl.Color == message.Color.ToString());
            if (found != null)
                Labels.Remove(found);
        }

        public void Handle(CardDetailChecklistViewModel.ChecklistAggregationsUpdated message)
        {
            Checklists = message.ChecklistCount;
            CheckItems = message.CheckItemsCount;
            CheckItemsChecked = message.CheckItemsCheckedCount;
        }

        public void Handle(CardDetailMembersViewModel.MemberAggregationsUpdated message)
        {
            Members = message.AssignedMemberCount;
        }

        [UsedImplicitly]
        public void GoToChecklists()
        {
            _eventAggregator.Publish(new CardDetailPivotViewModel.PivotRequestingNavigation
            {
                Screen = CardDetailPivotViewModel.Screen.Checklists
            });
        }

        [UsedImplicitly]
        public void GoToAttachments()
        {
            _eventAggregator.Publish(new CardDetailPivotViewModel.PivotRequestingNavigation
            {
                Screen = CardDetailPivotViewModel.Screen.Attachments
            });
        }

        [UsedImplicitly]
        public void GoToMembers()
        {
            _eventAggregator.Publish(new CardDetailPivotViewModel.PivotRequestingNavigation
            {
                Screen = CardDetailPivotViewModel.Screen.Members
            });
        }

        [UsedImplicitly]
        public void ChangeDueDate()
        {
            var model = new ChangeCardDueViewModel(GetView(), _eventAggregator, Id, Due);
            _windowManager.ShowDialog(model);
        }

        [UsedImplicitly]
        public void ChangeDescription()
        {
            var model = new ChangeCardDescriptionViewModel(GetView(), _eventAggregator)
            {
                CardId = Id,
                Description = Desc
            };

            _windowManager.ShowDialog(model);
        }

        [UsedImplicitly]
        public void ChangeLabels()
        {
            IEnumerable<Color> selected = Labels.Select(lbl => (Color) Enum.Parse(typeof (Color), lbl.Color));

            ChangeCardLabelsViewModel model = new ChangeCardLabelsViewModel(GetView(), _eventAggregator, _api, _progress)
            {
                CardId = Id
            }
                .Initialize(selected);

            _windowManager.ShowDialog(model);
        }

        [UsedImplicitly]
        public void DeleteCard()
        {
            MessageBox.Show("We should delete the card now.");
        }
    }
}