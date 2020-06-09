function bentoResource($http, umbRequestHelper) {

	return {
		getLibraryFolderId: function (contentId, libraryFolderDoctypeAlias) {
			return umbRequestHelper.resourcePromise(
				$http.get("/Umbraco/backoffice/Api/BentoResource/GetLibraryFolderId/", { params: { contentId: contentId, libraryFolderDoctypeAlias: libraryFolderDoctypeAlias } }),
				'Failed to retrieve the Bento Library Folder'
			);
		},

		getItemTypeFolderId: function (libraryFolderId, itemTypeFolderDoctypeAlias, selectedTypeName) {
			return umbRequestHelper.resourcePromise(
				$http.get("/Umbraco/backoffice/Api/BentoResource/GetItemTypeFolderId/", { params: {
					libraryFolderId: libraryFolderId,
					itemTypeFolderDoctypeAlias: itemTypeFolderDoctypeAlias,
					selectedTypeName: selectedTypeName
				}}),
				'Failed to retrieve the Bento Item Type Folder'
			);
		},

		getAllowedContentTypes: function (allowedDoctypeAliases) {
			return umbRequestHelper.resourcePromise(
				$http.get("/Umbraco/backoffice/Api/BentoResource/GetAllowedContentTypes/", { params: { allowedDoctypeAliases: allowedDoctypeAliases } }),
				'Failed to retrieve the allowed content types'
			);
		},

		getAllowedElementTypes: function (allowedElementAliases) {
			return umbRequestHelper.resourcePromise(
				$http.get("/Umbraco/backoffice/Api/BentoResource/GetAllowedElementTypes/", { params: { allowedElementAliases: allowedElementAliases } }),
				'Failed to retrieve the allowed elementtypes types'
			);
		},

		

		getRelationsByChildId: function (childId) {
			return umbRequestHelper.resourcePromise(
				$http.get("/Umbraco/backoffice/Api/BentoResource/GetRelationsByChildId/", { params: { childId: childId } }),
				'Failed to retrieve the relations'
			);
		}
	};
}
angular.module('umbraco.resources').factory('bentoResource', bentoResource);
