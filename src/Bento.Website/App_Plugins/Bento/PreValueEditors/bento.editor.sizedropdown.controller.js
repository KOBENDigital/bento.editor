function bentoBlockEditorSizeController($scope) {
	var vm = this;
	vm.editorSize = "small";

	if ($scope.model.value !== null && $scope.model.value !== "");
	vm.editorSize = $scope.model.value;

	function updateModel() {
		$scope.model.value = vm.editorSize;
	}

	$scope.$on("formSubmitting", function (ev, args) {
		updateModel();
	});
}

angular.module('umbraco').controller("bento.editor.sizedropdown.controller", bentoBlockEditorSizeController);