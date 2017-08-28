jQuery.postJSON = function (url, data, success, antiForgeryToken, dataType) {
    if (dataType === void 0) { dataType = "json"; }
    if (typeof (data) === "object") { data = JSON.stringify(data);}
    var ajax = {
        url: url,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: dataType,
        data: data,
        success: success
    };
    if (antiForgeryToken) {
        ajax.headers = {
            "__RequestVerificationToken": antiForgeryToken
        };
    };
    
    return jQuery.ajax(ajax);
};