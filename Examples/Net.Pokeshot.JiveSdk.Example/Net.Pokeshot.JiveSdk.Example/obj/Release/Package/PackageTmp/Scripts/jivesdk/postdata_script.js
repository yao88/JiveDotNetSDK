function PostDataViewModel() {
    var self = this;


    this.navigate = function (view, data) {
        console.log(view);
        console.log(data);
        navigate(view, data);
    }



    this.init = function () {
        console.log("Initializing home view");

   

        resizeApp();

    }

}


//Init the view and model
gadgets.util.registerOnLoadHandler(init);

function init() {

    console.log("home view");
    var model = new PostDataViewModel();


    model.init();

    ko.applyBindings(model);

}