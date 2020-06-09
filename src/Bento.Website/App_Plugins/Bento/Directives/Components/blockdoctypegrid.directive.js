(function () {
	'use strict';

	function blockDocTypeGridDirective() {

		function link(scope, el, attr, ctrl) {

			scope.clickItem = function (item, $event, $index) {
				if (scope.onClick) {
					scope.onClick(item, $event, $index);
				}
			};

			scope.clickItemName = function (item, $event, $index) {
				if (scope.onClickName && !($event.metaKey || $event.ctrlKey)) {
					scope.onClickName(item, $event, $index);
					$event.preventDefault();
				}
				$event.stopPropagation();
			};

		}

		var directive = {
			restrict: 'E',
			replace: true,
			templateUrl: '/App_Plugins/Bento/Views/Components/bento-block-doc-type-grid.html',
			scope: {
				content: '=',
				contentProperties: "=",
				onClick: "=",
				onClickName: "="
			},
			link: link
		};

		return directive;
	}

	angular.module('umbraco.directives').directive('bentoBlockDocTypeGrid', blockDocTypeGridDirective);
})();