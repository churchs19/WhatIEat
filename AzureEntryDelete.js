function del(id, user, request) {
	var entriesTable = tables.getTable('AzureEntry');

	entriesTable.where({
		UserId: user.userId,
		Id: id
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
			var deletedRecord = results[0];
			deletedRecord.EditDateTime = new Date();
			deletedRecord.IsDeleted = true;
			entriesTable.udate(deletedRecord, {
				success: function () {
					request.respond(statusCodes.OK, "");
				},
				error: function (err) {
					console.error("Error occurred for user '%s'. Details:", user.userId, err);
					request.respond(statusCodes.INTERNAL_SERVER_ERROR, "Unable to delete record.");
				}
			});
		} else {
			console.log('User %s attempted to update a record without permission.', user.userId);
			request.respond(statusCodes.FORBIDDEN, 'You do not have permission to update this record.');
		}
	}
}