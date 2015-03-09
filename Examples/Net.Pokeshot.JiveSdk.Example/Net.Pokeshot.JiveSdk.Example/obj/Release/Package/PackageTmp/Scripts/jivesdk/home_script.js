function HomeViewModel() {
    var self = this;
    this.currentUser = ko.observable();
    this.statusUpdate = ko.observable();

    this.navigate = function (view, data) {
        console.log(view);
        console.log(data);
        navigate(view, ko.toJS(data));
    }

    this.updateStatus = function () {
        console.log("our status update: " + self.statusUpdate());
        var request = osapi.jive.corev3.contents.create({
            "type":"update",
            "content": {
                "type": "text/html",
                "text": self.statusUpdate()
            }
            
        });

        request.execute(function (data) {
            console.log("Activity created!", data);
            self.currentUser().status(data.content.text)
        });


    }

    this.init = function () {
        console.log("Initializing home view");

        //We're getting the current user and binding some info to the UI
        var request = osapi.jive.corev3.people.get({ "id": "@me" });
        request.execute(function (response) {
            if (!response.error) {

                var user = response;
                //current user object
                console.log(user);
                self.currentUser(ko.mapping.fromJS(user));
                resizeApp();

            }
            else {
                console.log("Something bad happend");
            }
        });

     

    }

}


//Init the view and model
gadgets.util.registerOnLoadHandler(init);

function init() {

    console.log("home view");
    var model = new HomeViewModel();


    model.init();

    ko.applyBindings(model);

}