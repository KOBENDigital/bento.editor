(function () {
	'use strict';

	function LayoutPreviewDirective() {
		var directive = {
			restrict: 'E',
			replace: true,
			templateUrl: '/App_Plugins/Bento/Views/Components/bento-layout-preview.html',
			scope: {
				layout: "=",
				showMarkers: "="
			},
			controller: function ($scope) {
				$scope.gridColumns = '';
				$scope.updateColumns = updateColumns;
				$scope.getAreaStyles = getAreaStyles;

				function updateColumns(layout) {
					$scope.gridColumns = layout.columns.join(' ') + ";";
				}

				function getAreaStyles(area) {
					var style = "grid-column: " + area.column + ";";
					style += " ";
					style += "grid-row: " + area.row + ";";

					return style;
				}

				function layoutChanged() {
					$scope.updateColumns($scope.layout);
				}

				function init() {
					$scope.$watch('layout', layoutChanged, true);
				}

				init();
			}
		};

		return directive;
	}

	angular.module('umbraco.directives').directive('bentoLayoutPreview', LayoutPreviewDirective);
})();
