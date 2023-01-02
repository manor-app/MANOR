var uceBubbleChart;
var uceBubbleChartData;
var uceBuddleChartDataObject;
var uceBuddleChartDataObject2;
var uceBubbleChartOptions;

function initializeEceBubbleChart() {
    /*uceBubbleChartData = google.visualization.arrayToDataTable([
        ['ID', 'Delivrey-Day', 'Delivery-Month', 'Type', 'Project-ID'],
        [uceBuddleChartDataObject[0].Name, 21, 4, 'Project', uceBuddleChartDataObject[0].ID]
    ]);*/

    var currentYear = new Date().getFullYear();

    if (uceBuddleChartDataObject.length > 0) {
        var prEndDateDay = new Date(uceBuddleChartDataObject[0].EndDate).getDate();
        var prEndDateMonth = new Date(uceBuddleChartDataObject[0].EndDate).getMonth() + 1;
        uceBubbleChartData = google.visualization.arrayToDataTable([
            ['ID', 'Delivrey-Day', 'Delivery-Month', 'Type', 'Project-ID'],
            [uceBuddleChartDataObject[0].Name, prEndDateDay, prEndDateMonth, 'Project', uceBuddleChartDataObject[0].ID]
        ]);

        for (var i = 1; i < uceBuddleChartDataObject.length; i++) {

            if (currentYear != new Date(uceBuddleChartDataObject[i].EndDate).getFullYear())
                continue;

            if (uceBuddleChartDataObject[i].EndDate == null)
                continue;

            prEndDateDay = new Date(uceBuddleChartDataObject[i].EndDate).getDate();
            prEndDateMonth = new Date(uceBuddleChartDataObject[i].EndDate).getMonth() + 1;
            uceBubbleChartData.addRows([[uceBuddleChartDataObject[i].Name, prEndDateDay, prEndDateMonth, 'Project', uceBuddleChartDataObject[i].ID]]);
        }

    } else {
        uceBubbleChartData = google.visualization.arrayToDataTable([
            ['ID', 'Delivrey-Day', 'Delivery-Month', 'Type', 'Project-ID'],
            ['', 0, 0, 'Project', 0],
            ['', 0, 0, 'Sprint', 0]
        ]);
    }
    var beforeSprint = "";
    if (uceBuddleChartDataObject2.length > 0) {
        for (var i = 0; i < uceBuddleChartDataObject2.length; i++) {

            if (currentYear != new Date(uceBuddleChartDataObject2[i].EndDate).getFullYear())
                continue;

            if (uceBuddleChartDataObject2[i].Status == "Closed") {
                beforeSprint = "Closed-";
            }
            else if (uceBuddleChartDataObject2[i].Status == "Overdue") {
                beforeSprint = "Overdue-";
            } else {
                beforeSprint = "";
            }

            if (uceBuddleChartDataObject2[i].EndDate == null)
                continue;

            prEndDateDay = new Date(uceBuddleChartDataObject2[i].EndDate).getDate();
            prEndDateMonth = new Date(uceBuddleChartDataObject2[i].EndDate).getMonth() + 1;
            uceBubbleChartData.addRows([[uceBuddleChartDataObject2[i].Name, prEndDateDay, prEndDateMonth, beforeSprint+'Sprint', uceBuddleChartDataObject2[i].ProjectID]]);
        }
    }

    /*for (var i = 1; i < uceBuddleChartDataObject.length; i++) {
        uceBubbleChartData.addRows([[uceBuddleChartDataObject[i].Name, i*4, i*1.5, 'Project', uceBuddleChartDataObject[i].ID]]);
    }*/

    uceBubbleChartOptions = {
        title: currentYear,
        tooltip: {
            trigger: 'selection',
            isHtml: false,
            textStyle: {fontSize: 12}
        },
        hAxis: {
            title: 'Days', baseline: 1, maxvalue: 31, minvalue: 1, viewWindow: { min: 1, max: 31 },
            ticks: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31],
            textStyle: { fontSize: 12}
        },
        vAxis: {
            title: 'Months', baseline: 1, maxvalue: 12, minvalue: 1, viewWindow: { min: 1, max: 13 },
            ticks: [
                { v: 1, f: 'Jan' }, { v: 2, f: 'Feb' }, { v: 3, f: 'Mar' },
                { v: 4, f: 'Apr' }, { v: 5, f: 'May' }, { v: 6, f: 'Jun' },
                { v: 7, f: 'Jul' }, { v: 8, f: 'Aug' }, { v: 9, f: 'Sep' },
                { v: 10, f: 'Oct' }, { v: 11, f: 'Nov' }, { v: 12, f: 'Dec' }, { v: 13, f: '' }
            ],
            textStyle: { fontSize: 12 }
        },
        bubble: { textStyle: { fontSize: 11 } },
        legend: { textStyle: { fontSize: 11 }},
        sizeAxis: { maxSize: 12, minSize: 12 }/*,
        animation: {
            'startup': true,
            duration: 700,
            easing: 'out'
        }*/
    };

    uceBubbleChart = new google.visualization.BubbleChart(document.getElementById('uce-BubbleChart-Container'));

    function selectHandler() {
        var selectedItem = uceBubbleChart.getSelection()[0];
        if (selectedItem) {
            var topping = uceBubbleChartData.getValue(selectedItem.row, 4);
            //alert('The user selected ' + topping);
            //alert('The user selected ' + selectedItem.row);
        }
    }

    google.visualization.events.addListener(uceBubbleChart, 'select', selectHandler);

    uceBubbleChart.setAction({
        id: 'VisProj',
        text: 'GoTo Project',
        action: function () {
            selection = uceBubbleChart.getSelection();
            var topping = uceBubbleChartData.getValue(selection[0].row, 4);
            window.location = "/Project/ProjectPage?projectID=" + topping;
        }
    });

    uceBubbleChart.draw(uceBubbleChartData, uceBubbleChartOptions);
}
/*
window.onresize = function () {
    initializeEceBubbleChart();
    adjustListMargins();
}
*/

document.addEventListener("keydown", function(event) {

        if (event.keyCode == 27) {


            event.stopPropagation();
            event.preventDefault();

        }
        if (event.ctrlKey && event.altKey && event.keyCode == 82) {

        }

    

});

document.addEventListener("click", function(e) {
    //alert(e.target.parentNode.id);
    //if(e.target.id != "LIUIContainer" || e.target.parentNode.id != "LIUIContainer")
        //$('#LIUIContainer').removeClass("active");
    /*if($('#LIUIContainer').hasClass("active"))
           $('#LIUIContainer').toggleClass("active");*/

});


var bgCounter = 1;
function switchBg(){
    /*if(bgCounter > 5){
        bgCounter = 1;
        return;
    }
    $('body').css('background','url("/Images/bg'+bgCounter+'.jpg")');
    bgCounter++;*/
}

var elem = document.documentElement;
function openFullscreen() {
  if (elem.requestFullscreen) {
    elem.requestFullscreen();
  } else if (elem.webkitRequestFullscreen) { /* Safari */
    elem.webkitRequestFullscreen();
  } else if (elem.msRequestFullscreen) { /* IE11 */
    elem.msRequestFullscreen();
  }
}
function closeFullscreen() {
  if (document.exitFullscreen) {
    document.exitFullscreen();
  } else if (document.webkitExitFullscreen) { /* Safari */
    document.webkitExitFullscreen();
  } else if (document.msExitFullscreen) { /* IE11 */
    document.msExitFullscreen();
  }
}

/*$('document').ready(function() {
    //$(".dropdown-toggle").dropdown();
});*/

