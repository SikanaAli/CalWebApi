

$(() => {
    let BWI = $(".breadcrumb-wrapper-inner");
    let locationPath = window.location.pathname;
    let locationItems = locationPath.split('/')

    locationItems.splice(0, 1);
    BWI.empty();
    if (locationItems[0] == "") {
        BWI.append(`<a href="/" class="breadcrumb">Home</a>`);
    } else {
        locationItems.forEach((item, i) => {
            if (i === 0) {
                BWI.html(`<a href="/" class="breadcrumb">Home</a>`)
            } else {
                BWI.html(BWI.html()+`<a href="/${locationItems[0]/item}" class="breadcrumb">${item}</a>`)
            }
        })
    }

})