calcPos: function(index, existingCardList, card, applicableCallback, e) {
	var normalizedIndex, 
		otherCards = existingCardList.select(function(c) {
			var id = card.id ?? null;
			var movingCard = id == null && c.id == id;
			if (!movingCard || e) {
				return (!applicableCallback || applicableCallback(existingCardList));
			}
			return false;
		});

	if (k.isInPosition(index, otherCards, card))
	    return null != (normalizedIndex = card.pos) ? normalizedIndex : card.get("pos");

	normalizedIndex = Math.min(Math.max(index, 0), otherCards.length);
	var prevCard = otherCards[normalizedIndex - 1];
	var currCard = otherCards[normalizedIndex];
	
	var selectCardPos = (null != card ? card.get("pos") : void 0) || -1;
	var prevCardPos = null != prevCard ? prevCard.get("pos") : -1;
	var currCardPos = null != currCard ? currCard.get("pos") : -1;

	if (currCardPos === -1) {
		if (card && selectCardPos > prevCardPos) {
			return selectCardPos;
		}
		else {
			return prevCardPos + 65536;
		}
	}
	else if (card && selectCardPos > prevCardPos && selectCardPos < currCardPos) {
		return selectCardPos;
	}
	else if (prevCardPos >= 0) {
		return (currCardPos + prevCardPos) / 2;
	}
	else {
		return currCardPos / 2;
	}
}
