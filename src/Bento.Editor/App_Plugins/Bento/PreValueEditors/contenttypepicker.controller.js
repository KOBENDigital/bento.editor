function SingleContentTypePickerController($scope, contentTypeResource, editorService, angularHelper) {
	var vm = this;
	vm.loading = false;
	vm.contentTypes = [];
	vm.remove = remove;
	vm.add = add;
	vm.hasValue = false;

	var allContentTypes = null;

	function init() {
		vm.loading = true;
		vm.hasValue = $scope.model.value !== null && $scope.model.value !== "";
		contentTypeResource.getAll().then(function (all) {
			allContentTypes = all;
			vm.loading = false;
			// the model value is a comma separated list of content type aliases
			var currentContentTypes = _.map(($scope.model.value || "").split(","), function (s) { return s.trim(); });
			vm.contentTypes = _.filter(allContentTypes, function (contentType) {
				return currentContentTypes.indexOf(contentType.alias) >= 0;
			});
		});
	}

	function add() {
		editorService.contentTypePicker({
			multiPicker: true,
			submit: function (model) {
				var newContentTypes = _.map(model.selection, function (selected) {
					return _.findWhere(allContentTypes, { udi: selected.udi });
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

		if (angularHelper.getCurrentForm($scope) !== null) {
			angularHelper.getCurrentForm($scope).$setDirty();
		}
	}

	init();
}

angular.module('umbraco').controller("Bento.PrevalueEditors.ContentTypePickerController", SingleContentTypePickerController);