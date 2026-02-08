$(function () {
    setTimeout(function () {
        $('body').removeClass('loading');
    }, 1000);


    
});

document.getElementById("backToPreviousPage").addEventListener('click', () => {
    window.history.back();
})

