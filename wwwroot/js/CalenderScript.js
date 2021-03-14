
function selectDate(date) {
    $('#calendar').updateCalendarOptions({
        date: date
    });
    console.log("Single Click");
}

function onDoubleClick(date) {
    $('#calendar').updateCalendarOptions({
        date: date
    });
    $('#calendar-modal').iziModal('open');
    console.log("Doublick done");
}

var defaultConfig = {
    weekDayLength: 1,
    date: new Date(),
    onClickDate: selectDate,
    onDoubleClickDate: onDoubleClick,
    showYearDropdown: true,
};

$('#calendar').calendar(defaultConfig);
$('.day').addClass('noselect');


//Calender Modal
$("#calendar-modal").iziModal({
    title:"<strong>Schedule New Task</strong>",
    padding: "2rem",
    radius: 5,
    width:"60%",
    fullscreen: true,
    theme: "light",
    appendToOverlay: 'body',
    
    left: 0,
    right: 0,
    
});


//Form Validattion

jQuery.validator.setDefaults({
    debug: true,
    success: "valid"
});
$("#SubmitTask").click((e) => {
    e.preventDefault();
    console.log(e);
});