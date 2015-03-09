function ReadDataViewModel() {
    var self = this;
    this.values = ko.observableArray();


    this.navigate = function (view, data) {
        console.log(view);
        console.log(data);
        navigate(view, data);
    }



    this.init = function () {
        console.log("Initializing read data view");

        //Read some data from the server and display it in the UI
        getRequest(API_URL + "example/get", "get values",
        function (response) {
            console.log(response);
            self.values.pushAll(response);

        }, null);

    }

}


//Init the view and model
gadgets.util.registerOnLoadHandler(init);

function init() {

    console.log("Read Data view");
    var model = new ReadDataViewModel();


    model.init();

    ko.applyBindings(model);

}