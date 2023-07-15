// ===========================  navbar-collapse =========================== 
var navbar = document.getElementById('navbar');
var mobileMenu = document.getElementById('mobile-menu');
var navbarHeight = navbar.clientHeight;
var searchNav = document.getElementById('navSearch');
mobileMenu.onclick = function () {
    var isClosed = navbar.clientHeight === navbarHeight;
    if (isClosed) {
        navbar.style.height = 'auto';
        $('#navbar').addClass('sticky');
    } else {
        navbar.style.height = null;
        $('#navbar').removeClass('sticky');
    }
}

// ===========================  navbar-search=========================== 

function navSearch() {
    document.getElementById("dropdown-search").classList.toggle("show");
}
window.onscroll = () => {
    document.getElementById("dropdown-search").classList.remove('show');
}


$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop()) {
            $('#navbar').addClass('sticky');
            $('#backtop').fadeIn();

        } else {
            $('#navbar').removeClass('sticky');
            $('#backtop').fadeOut();
        }
    });
    $('#backtop').click(function () {
        $('html,body').animate({
            scrollTop: 0
        }, 1000);
    });
    let about = document.getElementById('about--content');
    let logo = document.getElementById('logo');
    let content1= document.getElementById('content1');
    window.addEventListener('scroll', function () {
        let value = window.scrollY;
        content1.style.left = value * 5.25 + 'px'
    }, 1000)

});