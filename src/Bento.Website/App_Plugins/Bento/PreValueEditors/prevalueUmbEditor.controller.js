(function () {
	'use strict';

	function editorDirective() {
		var directive = {
			restrict: 'E',
			replace: true,
			templateUrl: '/app_plugins/bento/views/prevalueeditors/prevalue-umb-editor.html',
			scope: {
				model: "="
			}
		};

		return directive;
	}

	angular.module('umbraco.directives').directive('prevalueUmbEditor', editorDirective);
})();