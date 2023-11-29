function SingleContentTypePickerController($scope, contentTypeResource, elementTypeResource, editorService, angularHelper) {
	var vm = this;
	vm.loading = false;
	vm.contentTypes = [];
	vm.remove = remove;
	vm.add = add;
	vm.hasValue = false;

	var allContentAndElementTypes = null;

	function init() {
		vm.loading = true;
		var allContentTypes = null;
		var allElementTypes = null;
		contentTypeResource.getAll().then(function (contentTypes) {
			allContentTypes = contentTypes;
			elementTypeResource.getAll().then(function (elementTypes) {
				allElementTypes = elementTypes;
				allContentAndElementTypes = allContentTypes.concat(allElementTypes);
				// the model value is a comma separated list of content type aliases
				var currentContentTypes = _.map(($scope.model.value || "").split(","), function (s) { return s.trim(); });
				vm.contentTypes = _.filter(allContentAndElementTypes, function (contentType) {
					return currentContentTypes.indexOf(contentType.alias) >= 0;
				});
				//ensure the document type alias has not changed
				vm.hasValue = vm.contentTypes.length > 0;
				vm.loading = false;
			});
		});
	}

	function add() {
		editorService.contentTypePicker({
			multiPicker: false,
			submit: function (model) {
				var newContentTypes = _.map(model.selection, function (selected) {
					//items return with a udi and a key, elements return with just a key - why?!?! they used to do both!
					return _.findWhere(allContentAndElementTypes, { key: selected.key });
				});
				vm.contentTypes = _.uniq(_.union(vm.contentTypes, newContentTypes));
				updateModel();
				editorService.close();
			},
			close: function () {
				editorService.close();
			}
		});
	}

	function remove(contentType) {
		vm.contentTypes = _.without(vm.contentTypes, contentType);
		updateModel();
	}

	function updateModel() {
		// the model value is a comma separated list of content type aliases
		$scope.model.value = _.pluck(vm.contentTypes, "alias").join();
		vm.hasValue = $scope.model.value !== null && $scope.model.value !== "";

		if (angularHelper.getCurrentForm($scope)) {
			angularHelper.getCurrentForm($scope).$setDirty();
		}
	}

	init();
}

angular.module('umbraco').controller("Bento.PrevalueEditors.SingleContentTypePickerController", SingleContentTypePickerController);