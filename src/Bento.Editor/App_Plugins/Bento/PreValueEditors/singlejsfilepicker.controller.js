function SingleJsFilePickerController($scope, editorService, angularHelper) {
	var vm = this;
	vm.loading = false;
	vm.file = "";
	vm.remove = remove;
	vm.add = add;
	vm.hasValue = false;


	function init() {
		vm.hasValue = $scope.model.value !== null && $scope.model.value !== "";
		vm.file = $scope.model.value;
	}

	function add() {

		const filePicker = {
			title: "Pick your required JS file",
			section: "settings",
			treeAlias: "files",
			entityType: "file",
			isDialog: true,
			filter: function (i) {
				return !(i.name.indexOf(".js") !== -1);
			},
			filterCssClass: "not-allowed",
			select: function (node) {
				const filepath = decodeURIComponent(node.id.replace(/\+/g, " "));
				vm.file = "/" + filepath;
				$scope.model.value = vm.file;

				updateModel();
				editorService.close();
			},
			close: function () {
				editorService.close();
			}
		};
		editorService.treePicker(filePicker);

	}

	function remove() {
		$scope.model.value = "";
		vm.file = "";
		updateModel();
	}

	function updateModel() {
		vm.hasValue = $scope.model.value !== null && $scope.model.value !== "";

		if (angularHelper.getCurrentForm($scope)) {
			angularHelper.getCurrentForm($scope).$setDirty();
		}
	}

	init();
}

angular.module('umbraco').controller("Bento.PrevalueEditors.SingleJsFilePickerController", SingleJsFilePickerController);