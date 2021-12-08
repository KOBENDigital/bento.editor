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
				updating: '=',
				culture: '='
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
					return  false;
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
						submit: function (model) {


							if (model.embed) {


								var value = buildEmbeddedContentData(model.node);

								$scope.contentData = value; // this is what we will save
								$scope.contentNode = model.node;

							} else {
								$scope.contentNode = model.contentNode;
							}

							$scope.id = $scope.contentNode.id;
							$scope.key = $scope.contentNode.key;
							$scope.updating = true;
							initBlock();
							

						},
						close: function (model) {
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
						config: $scope.config,
						submit: function (model) {


							var value = buildEmbeddedContentData(model.node);

							$scope.contentData = value; // this is what we will save
							$scope.contentNode = model.node;


							$scope.id = $scope.contentNode.id;
							$scope.key = $scope.contentNode.key;

							$scope.updating = true;
							initBlock();


						},
						close: function (model) {
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
							nodeData: $scope.contentData,
							documentTypeAlias: $scope.contentData.contentTypeAlias,
							documentTypeName: $scope.contentData.name,
							view: '/App_Plugins/Bento/bento.edit.html',
							submit: function (model) {

								
								var value = buildEmbeddedContentData(model.node);

								$scope.contentData = value; // this is what we will save
								$scope.contentNode = model.node;
								$scope.id = $scope.contentNode.id; // this is going to be 0
								$scope.key = $scope.contentNode.key;

								$scope.updating = true;
								initBlock();

								editorService.close();

							},
							close: function (model) {
								editorService.close();
							}
						};

						editorService.open(options);

					} else {

						options = {
							id: $scope.id,
							allowSaveAndClose: true,
							allowPublishAndClose: true,

							submit: function (model) {

								$scope.contentNode = model.contentNode;
								$scope.key = $scope.contentNode.key;
								$scope.id = $scope.contentNode.id;
								$scope.updating = true;
								initBlock();
								editorService.close();
							},
							close: function (model) {
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
										submit: function (model) {
											//todo: do we need to be passing the whole node around? i reckon the id'd be enough...										

											$scope.contentNode = model.selection[0];

											$scope.id = $scope.contentNode.id;
											$scope.key = $scope.contentNode.key;
											$scope.updating = true;
											initBlock();
											editorService.close();
											//$scope.model.close();
										},
										close: function () {
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
					$scope.id = ent.id;
					$scope.key = ent.key;

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

				

					// deal with the embeded block
					if (parseInt($scope.id) === 0) {

						$scope.embedded = true;
						setWrapperEmbedded($scope.contentData);

						let data = {
							guid: guid(),
							contentTypeAlias: $scope.contentTypeAlias,
							dataJson: JSON.stringify($scope.contentData),
							culture: typeof ($scope.culture) !== 'undefined' ? $scope.culture : null
						};


						let url = '/umbraco/backoffice/Api/Bento/LoadEmbeddedContent';

						$http.post(url, data).then(function (response) {

							$scope.getView = function () {

								
									return $sce.trustAsHtml(response.data);
								
							};

						}).catch(function (error) {
							console.log(error);

						}).finally(function () {
							$scope.updating = false;
						});

						setWatch();

						// else we want to load an existing one
					} else if ($scope.id !== undefined) {

						$scope.embedded = false;
						contentResource.getById($scope.id).then(setWrapper);

						//mvc view
						let url = '/umbraco/backoffice/Api/Bento/LoadLibraryContent?id=' + $scope.id;
						if (typeof ($scope.culture) !== 'undefined') {
							url += '&culture=' + $scope.culture;
						}

						$http.get(url).then(function (response) {

							$scope.getView = function () {
								return $sce.trustAsHtml(response.data);
							};

						}).catch(function (error) {
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

				init();

				initBlock();
			}
		};

		return directive;
	}

	angular.module('umbraco.directives').directive('bentoBlockPicker', bentoBlockPickerDirective);
})();