function parseMSJsonDate(jsonDate) {
    if (typeof (jsonDate) != "undefined" && jsonDate != null) {
        return new Date(parseInt(jsonDate.substr(6)));
    }
    else
        return null;
}

function formatChineseDate(date) {
    if (date != null) {
        var year = date.getFullYear(),
            month = date.getMonth() + 1,
            day = date.getDate();
        return year + "-" + month + "-" + day;
    }
    else
        return null;
}

function formatJsonDate(jsonDate) {
    var date = parseMSJsonDate(jsonDate);
    return formatChineseDate(date);
}

// This code courtesy http://ionfist.com/javascript/clearing-form-elements-with-jquery/
function clearForm(form) {
    $(form + " input," + form + " select," + form + " textarea").val("");
    $(form + " input[type=radio]," + form + " input[type=checkbox]").each(function () {
        this.checked = false;
        // Or
        // $(this).attr('checked', false);
    });
}

function renderTemplatedItems(data, templateId, containerSelector) {
    try
    {
        $(containerSelector).empty();
        $("#" + templateId).tmpl(data).appendTo($(containerSelector));
    }
    catch(err)
    {
    }
}

function ajaxFileUpload(animSelector, fileElementId, handlerUrl) {
    var asyncOp = $.Deferred();
    //starting setting some animation when the ajax starts and completes
    $(animSelector)
		.ajaxStart(function () {
		    $(this).show();
		})
		.ajaxComplete(function () {
		    $(this).hide();
		});

    $.ajaxFileUpload(
		{
			url: handlerUrl == null ? "UploadHandler.ashx" : handlerUrl,
			secureuri: false,
			fileElementId: fileElementId,
			dataType: 'json',
			success: function (data, status) {
			    if (typeof (data.error) != 'undefined') {
			        if (data.error != '') {
			            alert(data.error);
			        } else {
			            alert(data.msg);
			        }
			    }
			    asyncOp.resolve();
			},
			error: function (data, status, e) {
			    alert(e);
			}
		}
    )
    return asyncOp.promise();
}