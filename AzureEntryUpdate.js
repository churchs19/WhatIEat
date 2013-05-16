function update(item, user, request) {
	var entriesTable = tables.getTable('AzureEntry');

	entriesTable.where({
		UserId: user.userId,
		Id: item.Id
	}).read({
		success: updateEntry,
		error: function (err) {
			console.error("Error occurred for user '%s'. Details:", user.userId, err);
			request.respond(statusCodes.INTERNAL_SERVER_ERROR, "Unable to read table.");
		}
	});

	function updateEntry(results) {
		if (results.length > 0) {
			// Matching record for user was found. Continue normal execution.
			item.EditDateTime = new Date();
			request.execute();
		} else {
			console.log('User %s attempted to update a record without permission.', user.userId);
			request.respond(statusCodes.FORBIDDEN, 'You do not have permission to update this record.');
		}
	}
}