function insert(item, user, request) {
	item.UserId = user.userId;
	item.EdtDateTime = new Date();
	console.info(item);
	request.execute();
}