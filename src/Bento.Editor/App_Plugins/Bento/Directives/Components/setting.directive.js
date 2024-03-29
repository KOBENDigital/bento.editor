﻿(function () {
	'use strict';

	function bentoSettingDirective() {
		return {
			restrict: 'E',
			replace: true,
			templateUrl: '/App_Plugins/Bento/Views/Components/bento-setting.html',
			scope: {
				setting: "="
			}
		};
	}

	angular.module('umbraco.directives').directive('bentoSetting', bentoSettingDirective);
})();