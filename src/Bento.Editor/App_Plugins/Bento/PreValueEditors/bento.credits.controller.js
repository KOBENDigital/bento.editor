function bentoCreditsController($scope, $sce) {
	var vm = this;

	
	function init() {

		if ($scope.model.value) {
			vm.html = $sce.trustAsHtml($scope.model.value);
		}


	}

	init();
}

angular.module("umbraco").controller("bento.credits.controller", bentoCreditsController);