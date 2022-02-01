(function () {
	"use strict";

	function controller($location, editorState, userService, bentoResource, dateHelper) {

		var vm = this;
		vm.page = {};
		vm.page.loading = false;
		vm.CurrentNodeId = editorState.current.id;
		vm.CurrentNodeAlias = editorState.current.contentTypeAlias;

		vm.relationships = []; 

		vm.options = {
			includeProperties: [
				{ alias: "timestampFormatted", header: "Last Edited" },
				{ alias: "createdBy", header: "created By" }
			]
		};

		vm.selectItem = selectItem;
		vm.clickItem = clickItem;
		vm.allowSelectAll = false;
		vm.selectAll = selectAll;
		vm.isSelectedAll = isSelectedAll;
		vm.isSortDirection = isSortDirection;
		vm.sort = sort;

		function init() {
			vm.page.loading = true;

			bentoResource.getRelationsByChildId(vm.CurrentNodeId)
				.then(function (relations) {
					vm.page.loading = false;

					var items = [];
					for (var key in relations) {
						if (relations.hasOwnProperty(key)) {
							items[items.length] = [key, processItems(relations[key])];
						}
					}

					vm.relationships = relations;
				});
		}

		function processItems(relations) {
			if (relations) {
				//feels kinda clunky to do this in the view, maybe better to do on the server side?
				//but this is how the rest of the backoffice does it...
				userService.getCurrentUser().then(function (currentUser) {
					angular.forEach(relations, function (relation) {
						relation.timestampFormatted = dateHelper.getLocalDate(relation.lastEdited, currentUser.locale, 'YYYY-MM-DD HH:mm');
					});
				});
			}
		}

		function selectAll($event) {
		}

		function isSelectedAll() {

		}

		function clickItem(item) {
			//todo: hate that this path is hardcoded, just can't figure out how the hell to get it out of the backoffice code...
			//there's this in the 'listViewHelper':
			//createEditUrlCallback = function (item) {
			//    return "/" + $scope.entityType + "/" + $scope.entityType + "/edit/" + item.key + "?page=" + $scope.options.pageNumber + "&listName=" + $scope.contentId;
			//};
			$location.path("/content/content/edit/" + item.id);
		}

		function selectItem(selectedItem, $index, $event) {
			
		}

		function isSortDirection(col, direction) {

		}

		function sort(field, allow, isSystem) {

		}

		init();
	}

	angular.module("umbraco").controller("bento.relationshipManagerApp", controller);
})();