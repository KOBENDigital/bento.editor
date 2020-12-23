function JsCodeEditorController($scope, angularHelper) {
    var currentForm = angularHelper.getCurrentForm($scope);
    var vm = this;
    vm.script = "";

    if ($scope.model.value !== null && $scope.model.value !== "");
    vm.script = $scope.model.value;

    vm.aceOption = {
        mode: "javascript",
        theme: "chrome",
        minLines: 5,
        showPrintMargin: false,
        advanced: {
            fontSize: '16px',
            enableSnippets: true,
            enableBasicAutocompletion: true,
            enableLiveAutocompletion: false
        },
        onLoad: function (_editor) {

            vm.editor = _editor;

            //Update the auto-complete method to use ctrl+alt+space
            _editor.commands.bindKey("ctrl-alt-space", "startAutocomplete");

            //Unassigns the keybinding (That was previously auto-complete)
            //As conflicts with our own tree search shortcut
            _editor.commands.bindKey("ctrl-space", null);

            // TODO: Move all these keybinding config out into some helper/service
            _editor.commands.addCommands([
                //Disable (alt+shift+K)
                //Conflicts with our own show shortcuts dialog - this overrides it
                {
                    name: 'unSelectOrFindPrevious',
                    bindKey: 'Alt-Shift-K',
                    exec: function () {
                        //Toggle the show keyboard shortcuts overlay
                        $scope.$apply(function () {
                            vm.showKeyboardShortcut = !vm.showKeyboardShortcut;
                        });
                    },
                    readOnly: true
                }
            ]);


            vm.editor.on("change", changeAceEditor);
            vm.editor.on("blur", updateModel);

        }
    };

    function changeAceEditor() {
        
        setFormState("dirty");

    }

    function updateModel() {
        $scope.model.value = vm.script;
    }

    $scope.$on("formSubmitting", function (ev, args) {
        updateModel();
    });

    function setFormState(state) {

        // get the current form
        

        // set state
        if (state === "dirty") {
            currentForm.$setDirty();
        } else if (state === "pristine") {
            currentForm.$setPristine();
        }
    }

}

angular.module('umbraco').controller("Bento.PrevalueEditors.JsCodeEditorController", JsCodeEditorController);