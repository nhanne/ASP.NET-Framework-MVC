$(document).ready(function () {
    $(".detail__info-size label").on('click', function () {
        $(".detail__info-size label").removeClass('active');
        $(this).addClass('active');
    });
    $(".detail__info-color label").on('click', function () {
        $(".detail__info-color label").removeClass('active');
        $(this).addClass('active');
    });
    // about slide
    document.getElementById('next').onclick = function () {
        let lists = document.querySelectorAll('.item');
        document.getElementById('slide').appendChild(lists[0]);
    }
    document.getElementById('prev').onclick = function () {
        let lists = document.querySelectorAll('.item');
        document.getElementById('slide').prepend(lists[lists.length - 1]);
    }
    $('.cn_num').each(function () {
        $(this).prop('Counter', 0).animate({
            Counter: $(this).text()
        }, {
            duration: 4000,
            easing: 'swing',
            step: function (now) {
                $(this).text(Math.ceil(now));
            }
        });
    });


})