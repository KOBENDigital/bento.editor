(function () {
	'use strict';

	function bentoBlockPickerDirective(editorService, bentoResource, contentResource, userService, $routeParams, $sce, $http) {

		var directive = {
			restrict: 'E',
			templateUrl: '/App_Plugins/Bento/Views/Components/bento-block-picker.html',
			scope: {
				id: '=',
				key: '=',
				contentNode: '=',
				contentData: '=',
				contentName: '=',
				icon: '=',
				config: '=',
				area: '=',
				culture: '=',
				item: '=?',
				index: '=?'

			},

			controller: function ($scope, $element) {

				$scope.open = open;
				$scope.create = create;
				$scope.useExisting = useExisting;
				$scope.hasId = hasId;
				$scope.hasContentData = hasContentData;
				$scope.initBlock = initBlock;
				$scope.convert = convert;
				$scope.hasElements = hasElements;
				$scope.hasLibrary = hasLibrary;
				$scope.adminOnly = false;
				$scope.updating = false;

				$scope.allowedContentTypes = "";
				$scope.allowedElementTypes = "";

				function hasElements() {
					if ($scope.allowedElementTypes) {
						return true;
					}
					return false;
				}

				function hasLibrary() {
					if ($scope.config.libraryFolderDoctypeAlias !== null && $scope.allowedContentTypes) {
						return true;
					}
					return false;
				}

				function getPreviewUrl() {
					if ($scope.config.useFrontendFramework !== null) {
						if ($scope.config.useFrontendFramework) {
							return $scope.config.frontendFrameworkUrl;
						}
					}
					return '/umbraco/backoffice/Bento/LoadEmbeddedContent?contentid=' + $routeParams.id;
				}

				//hack to find out if user is admin
				userService.getCurrentUser().then(function (userObj) {
					var user = userObj;
					$scope.adminOnly = user.userGroups.indexOf("admin") >= 0;
				});

				function buildEmbeddedContentData(node) {

					var value = {
						name: node.contentTypeName,
						contentTypeAlias: node.contentTypeAlias,
						icon: node.icon
					};

					for (var v = 0; v < node.variants.length; v++) {
						var variant = node.variants[v];
						for (var t = 0; t < variant.tabs.length; t++) {
							var tab = variant.tabs[t];
							for (var p = 0; p < tab.properties.length; p++) {
								var prop = tab.properties[p];
								if (typeof prop.value !== "function") {
									value[prop.alias] = prop.value;
								}
							}
						}
					}

					return value;
				}

				function create(embed) {

					var options;
					options = {
						title: 'Bento block',
						view: '/App_Plugins/Bento/bento.create.html',
						embed: embed,
						config: $scope.config,
						size: 'medium',
						area: $scope.area,
						submit: (model) => {

							if (model.embed) {
								var value = buildEmbeddedContentData(model.node);
								$scope.item.contentData = value; // this is what we will save
								$scope.contentNode = model.node;
							} else {
								$scope.contentNode = model.contentNode;
							}

							$scope.item.id = $scope.contentNode.id;
							$scope.item.key = $scope.contentNode.key;
							$scope.item.index = $scope.index;

							initBlock();
							$scope.$emit("bentoSyncVal", $scope.item);
						},
						close: () => {
							editorService.close();
						}
					};

					editorService.open(options);
				}

				function convert(id) {
					var options;
					options = {
						title: 'Convert',
						view: '/App_Plugins/Bento/bento.convert.html',
						itemid: id,
						config: $scope.allowedElementTypes,
						submit: (model) => {

							var value = buildEmbeddedContentData(model.node);

							$scope.item.contentData = value; // this is what we will save
							$scope.contentNode = model.node;
							$scope.item.id = $scope.contentNode.id;
							$scope.item.key = $scope.contentNode.key;
							$scope.item.index = $scope.index;

							$scope.$emit("bentoSyncVal", $scope.item);
						},
						close: () => {
							editorService.close();
						}
					};

					editorService.open(options);
				}

				function open() {

					var options;

					if ($scope.embedded) {
						options = {
							title: 'Edit',
							embed: $scope.embedded,
							nodeData: $scope.item.contentData,
							documentTypeAlias: $scope.item.contentData.contentTypeAlias,
							documentTypeName: $scope.item.contentData.name,
							view: '/App_Plugins/Bento/bento.edit.html',
							submit: (model) => {
								var value = buildEmbeddedContentData(model.node);

								$scope.item.contentData = value; // this is what we will save
								$scope.contentNode = model.node;
								$scope.item.id = $scope.contentNode.id; // this is going to be 0
								$scope.item.key = $scope.contentNode.key;
								$scope.item.index = $scope.index;
								$scope.$emit("bentoSyncVal", $scope.item);

								editorService.close();
							},
							close: () => {
								editorService.close();
							}
						};

						editorService.open(options);

					} else {

						options = {
							id: $scope.item.id,
							allowSaveAndClose: true,
							allowPublishAndClose: true,

							submit: (model) => {
								$scope.contentNode = model.contentNode;
								$scope.item.key = $scope.contentNode.key;
								$scope.item.id = $scope.contentNode.id;
								$scope.item.index = $scope.index;

								$scope.$emit("bentoSyncVal", $scope.item);
								editorService.close();
							},
							close: () => {
								editorService.close();
							}
						};

						editorService.contentEditor(options);
					}
				}

				function useExisting() {
					bentoResource.getLibraryFolderId($routeParams.id, $scope.config.libraryFolderDoctypeAlias)
						.then(function (ent) {
							var libraryFolderId = ent;

							var contentPicker = {
								section: "content",
								treeAlias: "content",
								multiPicker: false,
								startNodeId: libraryFolderId,
								filterCssClass: 'not-allowed not-published',
								filter: $scope.allowedContentTypes,
								submit: (model) => {
									//todo: do we need to be passing the whole node around? i reckon the id'd be enough...
									$scope.contentNode = model.selection[0];

									$scope.item.id = $scope.contentNode.id;
									$scope.item.key = $scope.contentNode.key;
									$scope.item.index = $scope.index;

									$scope.$emit("bentoSyncVal", $scope.item);
									editorService.close();
								},
								close: () => {
									editorService.close();
								}
							};

							editorService.treePicker(contentPicker);
						});
				}

				function hasId(value) {
					return value !== null && parseInt(value, 10) > 0;
				}

				function hasContentData(value) {
					if (value !== undefined) {
						return true;
					}
					return false;
				}

				function setWrapperEmbedded(contentData) {
					$scope.contentTypeName = $scope.contentName = contentData.name;
					$scope.contentTypeAlias = contentData.contentTypeAlias;

					$scope.icon = contentData.icon;

					//not created
					$scope.status = {
						label: "Embedded",
						color: "gray",
						embedded: true
					};
				}

				function setWrapper(ent) {
					$scope.content = ent;
					$scope.item.id = ent.id;
					$scope.item.key = ent.key;
					$scope.item.index = $scope.index;

					$scope.contentTypeName = ent.contentTypeName;
					$scope.contentTypeAlias = ent.contentTypeAlias;
					//todo: this needs to switch when multiple languages are available
					var variant = ent.variants[0];
					$scope.contentName = variant.name !== '' ? variant.name : ent.contentTypeName;
					$scope.icon = ent.icon;
					$scope.state = variant.state;

					$scope.status = {};

					// variant status
					if (variant.state === "Draft") {
						// draft node
						$scope.status.label = "Draft";
						$scope.status.color = "gray";
					} else if (variant.state === "Published") {
						// published node
						$scope.status.label = "Published";
						$scope.status.color = "success";
					} else if (variant.state === "PublishedPendingChanges") {
						// published node with pending changes
						$scope.status.label = "Published (Pending Changes)";
						$scope.status.color = "success";
					}
				}

				function setWatch() {
					//set the control title in the grid editor active view
					$scope.$watch(function () {
						return $element.closest('.umb-control').attr('class');
					},
						function (el) {
							if (typeof el !== 'undefined' && el !== null && el.includes("active")) {
								var controlTitle = $element.closest(".umb-control-inner").find('.umb-control-title');
								if (controlTitle.length > 0) {
									controlTitle.text($scope.contentName);
								}
							}
						});

					//set the control title in the grid editor when sorting
					$scope.$watch(function () {
						return $element.closest('.umb-control').find('.umb-control-collapsed').attr('class');
					},
						function (el) {
							if (typeof el !== 'undefined' && el !== null && !el.includes("ng-hide")) {
								$element.closest('.umb-control').find('.umb-control-collapsed').text($scope.contentName);
							}
						});
				}

				function guid() {
					function s4() {
						return Math.floor((1 + Math.random()) * 0x10000)
							.toString(16)
							.substring(1);
					}
					return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
						s4() + '-' + s4() + s4() + s4();
				}

				function initBlock() {
					if (!$scope.item) {
						$scope.item = {
							id: $scope.id,
							key: $scope.key,
							index: $scope.index,
						};
						if ($scope.contentData) {
							$scope.item.contentData = $scope.contentData;
							$scope.item.icon = $scope.icon;
							$scope.item.contentNode = $scope.contentNode;
						}
					}

					// deal with the embeded block
					if ($scope.item && parseInt($scope.item.id) === 0 && $scope.item.contentData) {

						$scope.embedded = true;
						setWrapperEmbedded($scope.item.contentData);

						let data = {
							guid: $scope.item.key,
							contentTypeAlias: $scope.contentTypeAlias,
							dataJson: JSON.stringify($scope.item.contentData),
							culture: typeof ($scope.culture) !== 'undefined' ? $scope.culture : null
						};

						let url = getPreviewUrl();
						$scope.updating = true;
						$http.post(url, data).then(function (response) {

							var html = response.data;

							$scope.$broadcast("bentoSyncPreview", html);
						}).catch(function (error) {
							console.log('Error LoadEmbeddedContent');
							console.log(error);
						}).finally(function () {
							$scope.updating = false;
						});

						setWatch();

						// else we want to load an existing one
					} else if ($scope.item.id !== undefined && $scope.item.id > 0) {

						$scope.embedded = false;
						contentResource.getById($scope.item.id).then(setWrapper);

						//mvc view
						let url = '/umbraco/backoffice/Bento/LoadLibraryContent?id=' + $scope.id + '&contentid=' + $routeParams.id;

						if (typeof ($scope.culture) !== 'undefined') {
							url += '&culture=' + $scope.culture;
						}

						$scope.updating = true;

						$http.get(url).then(function (response) {
							var html = response.data;
							$scope.$broadcast("bentoSyncPreview", html);
						}).catch(function (error) {
							console.log('Error LoadLibraryContent');
							console.log(error);

						}).finally(function () {
							$scope.updating = false;
						});
						setWatch();
					}
				}

				function init() {

					//set layout allowed block 
					if ($scope.area !== undefined) {
						// this could probably be done better

						if ($scope.area.allowedElementTypes.value !== undefined && $scope.area.allowedElementTypes.value !== '') {
							$scope.allowedElementTypes = $scope.area.allowedElementTypes.value;
						}
						if (typeof $scope.area.allowedElementTypes === 'string' && $scope.area.allowedElementTypes) {
							$scope.allowedElementTypes = $scope.area.allowedElementTypes;
						}

						if ($scope.area.allowedContentTypes.value !== undefined && $scope.area.allowedContentTypes.value !== '') {
							$scope.allowedContentTypes = $scope.area.allowedContentTypes.value;
						}
						if (typeof $scope.area.allowedContentTypes === 'string' && $scope.area.allowedContentTypes) {
							$scope.allowedContentTypes = $scope.area.allowedContentTypes;
						}

						//set bento item allowed blocks
					} else {

						if ($scope.config.allowedDoctypeAliases) {
							$scope.allowedContentTypes = $scope.config.allowedDoctypeAliases;
						}

						if ($scope.config.allowedElementAliases) {
							$scope.allowedElementTypes = $scope.config.allowedElementAliases;
						}
					}
				}

				var unsubscribe = $scope.$on("bentoSyncVal", function (ev, args) {
					$scope.item = args;
					initBlock();
				});

				var unsubscribeClear = $scope.$on("bentoClearVal", function (ev, args) {
					if (args === $scope.index) {
						$scope.item = undefined;
						$scope.contentData = undefined;
						$scope.id = undefined;
						$scope.key = undefined;
						initBlock();
					}
				});

				$scope.$on('$destroy', function () {
					unsubscribe();
					unsubscribeClear();
				});

				this.$onInit = function () {
					init();
					initBlock();
				}
			}
		};

		return directive;
	}

	angular.module('umbraco.directives').directive('bentoBlockPicker', bentoBlockPickerDirective);
})();