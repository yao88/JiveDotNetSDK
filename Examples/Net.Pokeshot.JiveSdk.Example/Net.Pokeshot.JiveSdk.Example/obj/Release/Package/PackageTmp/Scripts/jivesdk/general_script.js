
/*
* Jive .Net SDK share JavaScript functions
* Copyright (c) SMZ SocialMediaZolutions GmbH & Co. KG
* All rights reserved.
*
* MIT License
* Permission is hereby granted, free of charge, to any person obtaining a copy of this
* software and associated documentation files (the ""Software""), to deal in the Software
* without restriction, including without limitation the rights to use, copy, modify, merge,
* publish, distribute, sublicense, and/or sell copies of the Software, and to permit
* persons to whom the Software is furnished to do so, subject to the following conditions:
* The above copyright notice and this permission notice shall be included in all copies or
* substantial portions of the Software.
* THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
* INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
* PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
* FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
* OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.

* This class contains the RegisterJiveInstance Method, which takes a registration request from Jive Add-on
 * upon installation, uninstallation, or re-connect and creates/updates the Jive Instance
 * and Jive Add-On entries in the database accordingly
* 
*/

gadgets.util.registerOnLoadHandler(initApp);
function initApp() {
    dataParams = gadgets.views.getParams();

    debug('START', 'received data', dataParams);
}

function resizeApp() {

    gadgets.window.adjustHeight();

}

function parseISO8601(dateStringInRange) {
    var isoExp = /^\s*(\d{4})-(\d\d)-(\d\d)T(\d\d):(\d\d):(\d\d).*\s*$/,
        date = new Date(NaN), month,
        parts = isoExp.exec(dateStringInRange);
    if (parts) {
        month = +parts[2];
        date.setUTCFullYear(parts[1], month - 1, parts[3]);
        date.setUTCHours(parts[4]);
        date.setUTCMinutes(parts[5]);
        date.setUTCSeconds(parts[6]);
        if (month != date.getMonth() + 1) {
            date.setTime(NaN);
        }
    }
    return date;
}

function getRequest(url, type, successHandler, errorHandler) {



    if (type != null) {

    }
    console.log('GET', url, null);
    osapi.http.get({
        'href': url,
        'format': 'json',
        'authz': 'signed'
    }).execute(function (response) {
        //console.log('response JSON  ', JSON.stringify(response));
        //console.log('response Object', response);
        //console.log(response.status);
        if (response.status >= 400) {
            if (errorHandler != null) {

                errorHandler(response);
            }
            else {
                showMessage("Loading data failed. Error message: " + response.error.message, "error");
                navigate("canvas");

            }
        }
        if (successHandler != null) {
            if (!response.error) {
                successHandler(response.content);


            }
        }
        if (type != null) {

        }
    });
}

function postRequest(url, data, successHandler, errorHandler) {
    console.log('POST', url, data);
    osapi.http.post({
        'href': url,
        'body': data,
        'headers': { 'Content-Type': ['application/json'] },
        'format': 'json',
        'authz': 'signed'
    }).execute(function (response) {
        console.log(null, 'response', response);
        if (response.error) {
            if (errorHandler != null) {
                if (response.error) {
                    errorHandler(response.error);
                } else {
                    errorHandler(response.content);
                }
            } else {
                showMessage("Data submit failed.", "error");
            }
        }
        if (successHandler != null) {
            if (!response.error) {
                successHandler(response.content);
            }
        }
    });
}


function getJiveContent(type, id, fields, handler, identifier) {
    if (id == undefined || id == null) {
        return;
    }

    /*
    if (id.toString().indexOf("@") >= 0) {
        id = id.substring(0, id.indexOf('@'));
    }
    */

    var request = null;
    var validJiveContentTypes = [JIVE_CONTENT_DISCUSSION, JIVE_CONTENT_DOCUMENT, 'file', 'poll', 'post', 'dm', 'favorite', 'task', 'update'];

    if (fields == null || !(typeof fields === 'string') || fields == "") {
        fields = "@summary";
    }

    if (type == JIVE_CONTENT_DISCUSSION) {
        debug('FETCHING DISCUSSION FOR SOCIALGROUP ' + id);

        request = osapi.jive.corev3.contents.get({
            type: JIVE_CONTENT_DISCUSSION,
            place: id,
            fields: '@all',
            count: 5
        });

    } else if (type == JIVE_CONTENT_GROUP) {
        //id = 1016;

        request = osapi.jive.corev3.places.get({
            // entityDescriptor: '700,' + id,
            "uri": "/places/" + id, //placesId
            fields: fields,
            count: 1
        });

    } else if (type == JIVE_CONTENT_USER) {
        request = osapi.jive.corev3.people.get({
            id: id,
            fields: fields
        })
    } else {
        if ($.inArray(type, validJiveContentTypes)) {
            request = osapi.jive.corev3.contents.get({
                type: type,
                id: id,
                fields: fields
            });
        }
    }

    if (request != null) {
        request.execute(function (response) {
            debug('JIVE', 'response', response);
            if (response.error) {
                debug('ERROR', response.error.code + ' reading jive content.  Error message was: ' + response.error.message, null);
            } else {
                if (handler != null) {
                    if (identifier != null) {
                        handler(response, identifier);
                    } else {
                        handler(response);
                    }
                }
            }
        });
    } else {
        if (handler != null) {
            if (identifier != null) {
                handler(null, identifier);
            } else {
                handler(null);
            }
        }
    }
}
function navigate(page) {

    var data;
    if (navigate.arguments.length > 1) {
        data = navigate.arguments[1];
    }
    else {
        data = new Object();
        data.title = "navigation";
    }

    console.log("current view: " + gadgets.views.getCurrentView().getName());
    try {
        console.log(data);
        data.navigatedFrom = gadgets.views.getCurrentView().getName();
        gadgets.views.requestNavigateTo(page, data);
        debug('NAV', 'navigated to view ' + page, null);
    }
    catch (err) {
        //nüx
        console.log(err);
    }
}

function debug(type, message, obj) {

    if (type == null) {
        type = "";
    }
    var output = type;
    while (output.length < 10) {
        output += " ";
    }
    if (message != null) {
        output += message + " ";
    }
    try {
        if (obj == null) {
            console.log(output);
        } else {
            console.log(output, obj);
        }
    } catch (err) {
        //unable to log to console...
    }
}

ko.observableArray.fn.pushAll = function (valuesToPush) {

    if (typeof valuesToPush === 'string') {
        valuesToPush = ko.utils.parseJson(JSONdataFromServer);
    }

    var underlyingArray = this();
    this.valueWillMutate();
    ko.utils.arrayPushAll(underlyingArray, valuesToPush);
    this.valueHasMutated();
    return this;
};

//Authorization popup window code
$.oauthpopup = function (options) {
    options.windowName = options.windowName || 'ConnectWithOAuth'; // should not include space for IE
    options.windowOptions = options.windowOptions || 'location=0,status=0,width=800,height=400';
    options.callback = options.callback || function () { window.location.reload(); };
    var that = this;
    var location = new Object();
    console.log(options.path);
    that._oauthWindow = window.open(options.path, options.windowName, options.windowOptions);
    that._oauthInterval = window.setInterval(function () {

        if (that._oauthWindow.closed) {

            window.clearInterval(that._oauthInterval);
            options.callback();
        }
    }, 1000);
};