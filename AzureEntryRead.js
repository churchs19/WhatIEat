function read(query, user, request) {
//	console.info(query);
	query.where({ UserId: user.userId });
	request.execute();
}