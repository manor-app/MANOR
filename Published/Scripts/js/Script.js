window.onresize = function() {

}

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

function homePageAct(){
    $('#startingPoint').addClass("active");
    setTimeout(function(){
        $('#projectActStage .stageName').addClass("active");
    },500);
    setTimeout(function(){
        $('#projectActStage .sfsSeparator').addClass("active");
    },2500);
    setTimeout(function(){
        $('#projectActStage .stageEnd').addClass("active");
    },3500);
    setTimeout(function(){
        $('#projectActStage .stageName').addClass("activeNext");
    },4500);
    setTimeout(function(){
        $('#serverActStage .stageName').addClass("active");
    },5500);
    setTimeout(function(){
        $('#serverActStage .sfsSeparator').addClass("active");
    },7500);
    setTimeout(function(){
        $('#serverActStage .stageEnd').addClass("active");
    },8500);
    setTimeout(function(){
        $('#serverActStage .stageName').addClass("activeNext");
    },9500);
    setTimeout(function(){
        $('#applicationActStage .stageName').addClass("active");
    },10500);
    setTimeout(function(){
        $('#applicationActStage .sfsSeparator').addClass("active");
    },12500);
    setTimeout(function(){
        $('#applicationActStage .stageEnd').addClass("active");
    },13500);
    setTimeout(function(){
        $('#applicationActStage .stageName').addClass("activeNext");
    },14500);
    setTimeout(function(){
        $('#endPoint').addClass("active");
    },15500);
    setTimeout(function(){
        $('#endCaption').addClass("active");
    },16500);
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

$('document').ready(function() {
    $(".dropdown-toggle").dropdown();
});
