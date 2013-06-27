using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using JetBrains.Annotations;
using trello.Interactions;
using trellow.api.Cards;

namespace trello.ViewModels
{
    public class CardViewModel : Screen
    {
        private readonly INavigationService _navigationService;
        private InteractionManager _interactionManager;

        public string Id { get; set; }

        public string Name { get; set; }

        public string BoardName { get; set; }

        public string ListName { get; set; }

        public string Desc { get; set; }

        public double Pos { get; set; }

        public DateTime? Due { get; set; }

        public int Votes { get; set; }

        public int Comments { get; set; }

        public int CheckItems { get; set; }

        public int CheckItemsChecked { get; set; }

        public int Attachments { get; set; }

        public string CoverUri { get; set; }

        public int CoverHeight { get; set; }

        public int CoverWidth { get; set; }

        public IObservableCollection<MemberViewModel> Members { get; set; }

        public IObservableCollection<LabelViewModel> Labels { get; set; }

        public CardViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            Members = new BindableCollection<MemberViewModel>();
            Labels = new BindableCollection<LabelViewModel>();
        }

        public CardViewModel InitializeWith(Card card, InteractionManager interactionManager = null)
        {
            _interactionManager = interactionManager;

            var cover = card.Attachments.SingleOrDefault(att => att.Id == card.IdAttachmentCover);

            BoardName = card.Board != null ? card.Board.Name : null;
            ListName = card.List != null ? card.List.Name : null;

            Id = card.Id;
            Name = card.Name;
            Desc = card.Desc;
            Due = card.Due;
            Votes = card.Badges.Votes;
            Comments = card.Badges.Comments;
            CheckItems = card.Badges.CheckItems;
            CheckItemsChecked = card.Badges.CheckItemsChecked;
            Attachments = card.Badges.Attachments;
            Pos = card.Pos;

            CoverUri = cover != null ? cover.Previews.First().Url : null;
            CoverHeight = cover != null ? cover.Previews.First().Height : 0;
            CoverWidth = cover != null ? cover.Previews.First().Width : 0;

            Members.Clear();
            Members.AddRange(card.Members.Select(mem => new MemberViewModel(mem)));

            Labels.Clear();
            Labels.AddRange(card.Labels.Select(lbl => new LabelViewModel(lbl.Color.ToString(), lbl.Name)));

            return this;
        }

        protected override void OnViewLoaded(object view)
        {
            var element = view as FrameworkElement;
            if (element != null && _interactionManager != null)
                _interactionManager.AddElement(element);
        }

        [UsedImplicitly]
        public void Open()
        {
            _navigationService.UriFor<CardDetailPivotViewModel>()
                .WithParam(x => x.Id, Id)
                .Navigate();
        }
    }
}