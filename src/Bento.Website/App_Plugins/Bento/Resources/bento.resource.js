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

		// Helpers (functions shared by bento-inserter and bentopicker)

		, generateUuid: function () {
			function s4() {
				return Math.floor((1 + Math.random()) * 0x10000)
					.toString(16)
					.substring(1);
			}
			return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
		}

		, buildEmbeddedContentData: function (node) {
			var value = {
				name: node.contentTypeName,
				contentTypeAlias: node.contentTypeAlias,
				icon: node.icon
			};

			for (var v = 0; v < node.variants.length; v++) {
				var variant = node.variants[v];
				for (var t = 0; t < variant.tabs.length; t++) {
					var tab = variant.tabs[t];
					for (var p = 0; p < tab.properties.length; p++) {
						var prop = tab.properties[p];
						if (typeof prop.value !== "function") {
							value[prop.alias] = prop.value;
						}
					}
				}
			}

			return value;
		}
	};
}
angular.module('umbraco.resources').factory('bentoResource', bentoResource);
