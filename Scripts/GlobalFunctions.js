function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

function handleTablePageChange(tableBodyObject,rowsCount,pageNum, itemsPerPage) {
    var start = (pageNum-1) * itemsPerPage + 1;
    var end = start + itemsPerPage - 1;

    for (var i = 0; i < rowsCount; i++) {
        var count = i + 1;
        if (count < start || count > end)
            tableBodyObject.children().eq(i).hide();
        else
            tableBodyObject.children().eq(i).fadeIn();
    }

}

function uploadFiles(fileInputElement, filePath) {
    // Checking whether FormData is available in browser  
    if (window.FormData !== undefined) {

        var fileUpload = $(fileInputElement).get(0);
        var files = fileUpload.files;

        // Create FormData object  
        var fileData = new FormData();

        // Looping over all files and add it to FormData object  
        for (var i = 0; i < files.length; i++) {
            fileData.append(files[i].name, files[i]);
        }

        // Adding one more key to FormData object  
        fileData.append('FilePath', filePath);

        $.ajax({
            url: '/Home/UploadFiles',
            type: "POST",
            contentType: false, // Not to set any content header  
            processData: false, // Not to process data  
            data: fileData,
            success: function (result) {
                //alert(result);
            },
            error: function (xhr, status, error) {
                alert("Error: " + xhr.responseText);
            }
        });
    } else {
        alert("FormData is not supported.");
    }
}

function adjustListMargins(listContainer = '#ProjectsList') {
    var listContainerHeight = ($(listContainer).css("height").replace("px", "")) - 75;
    var listItemsCount = $(listContainer + ' ul').children().length;
    var listItemHeight = $(listContainer + ' ul').children().eq(0).css("height").replace("px", "");
    var listItemsTotalHeight = listItemsCount * listItemHeight;
    var remainingListSpace = listContainerHeight - listItemsTotalHeight;
    var itemsMargin = remainingListSpace / listItemsCount;

    if (itemsMargin < 10) {
        itemsMargin = 10;
    }

    $(listContainer + ' ul').children().css("margin", "" + (itemsMargin * 1.1) + "px auto " + itemsMargin + "px auto");
    if (listContainer == "ProjectComponents") {
        $(listContainer + ' ul').children().eq(0).css("margin", "10px auto " + itemsMargin + "px auto");
    } else {
        $(listContainer + ' ul').children().eq(0).css("margin", "30px auto " + itemsMargin + "px auto");
    }
    $(listContainer + ' ul').children().eq($(listContainer + ' ul').children().length - 1).css("margin", "" + (itemsMargin*1.1) + "px auto 0px auto");
}

Element.prototype.UpdateTablePagination = function (tableBodyObjectSent = null, index = 0, itemsPerPage = 5) {

    if (this.className == "Table-Pagination") {

        var minPageNum = 1;
        var minPageItems = 5;
        var maxPageItems = 20;
        var pagesNumber = 1;

        var tableBodyObject = $(this).next().children().eq(1);

        if (tableBodyObjectSent != null)
            tableBodyObject = tableBodyObjectSent;

        var rowsCount = tableBodyObject.children().length;

        if (itemsPerPage == null) {
            itemsPerPage = minPageItems;
        }

        var pagesNumber = Math.ceil(rowsCount / itemsPerPage);
        var pagesDDM = $(this).children().eq(0).children().eq(0).children().eq(1);
        var itemsPerPageDDM = $(this).children().eq(0).children().eq(1).children().eq(1);

        pagesDDM.attr("onchange", "handleTablePageChange($(\"#" + tableBodyObject.attr("id") + "\"), " + rowsCount + ", $(this).val(), " + itemsPerPage + ");");

        itemsPerPageDDM.attr("onchange", "$('.Table-Pagination')[" + index + "].UpdateTablePagination($(\"#" + tableBodyObject.attr("id") + "\")," + index + ", $(this).val());");

        pagesDDM.html("");
        pagesDDM.append("<option selected value='1'>1</option>");

        for (var i = 2; i <= pagesNumber; i++) {
            pagesDDM.append("<option value='" + i + "'>" + i + "</option>");
        }

        itemsPerPageDDM.html("");
        for (var i = minPageItems; i <= maxPageItems; i += 5) {
            if (itemsPerPage == i) {
                itemsPerPageDDM.append("<option selected value='" + i + "'>" + i + "</option>");
            } else {
                itemsPerPageDDM.append("<option value='" + i + "'>" + i + "</option>");
            }
        }
        
        for (var i = 0; i < rowsCount; i++) {
            var count = i + 1;
            if (count > itemsPerPage)
                tableBodyObject.children().eq(i).hide();
            else
                tableBodyObject.children().eq(i).show();
        }
    }
}

var tableToExcel = (function () {
    var uri = 'data:application/vnd.ms-excel;base64,'
        , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--><meta http-equiv="content-type" content="text/plain; charset=UTF-8"/></head><body><table>{table}</table></body></html>'
        , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
        , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
    return function (table, name) {
        if (!table.nodeType) table = document.getElementById(table)
        var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
        window.location.href = uri + base64(format(template, ctx))
    }
})()