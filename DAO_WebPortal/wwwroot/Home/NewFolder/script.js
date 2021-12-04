const Dropzone = require("./jquery.bundle");

/* Template: UserCenter v1.0.1 @ICO Crypto */
(function($){
	'use strict';
	var $win = $(window), $body_m = $('body'), $navbar = $('.navbar');
	
	// Touch Class
	if (!("ontouchstart" in document.documentElement)) {
		$body_m.addClass("no-touch");
	}
	// Get Window Width
	function winwidth () {
		return $win.width();
	}
	var wwCurrent = winwidth();
	$win.on('resize', function () { 
		wwCurrent = winwidth(); 
	});

	// Sticky
	var $is_sticky = $('.is-sticky');
	if ($is_sticky.length > 0 ) {
		var $navm = $('#mainnav').offset();
		$win.scroll(function(){
			var $scroll = $win.scrollTop();
			if ($win.width() > 991) {
				if($scroll > $navm.top ){
					if(!$is_sticky.hasClass('has-fixed')) {$is_sticky.addClass('has-fixed');}
				} else {
					if($is_sticky.hasClass('has-fixed')) {$is_sticky.removeClass('has-fixed');}
				}
			} else {
				if($is_sticky.hasClass('has-fixed')) {$is_sticky.removeClass('has-fixed');}
			}
		});
	}
	
	// Active page menu when click
	var CurURL = window.location.href, urlSplit = CurURL.split("#");
	var $nav_link = $("a");
	if ($nav_link.length > 0) {
		$nav_link.each(function() {
			if (CurURL === (this.href) && (urlSplit[1]!=="")) {
				$(this).closest("li").addClass("active").parent().closest("li").addClass("active");
			}
		});
	}
    
	// Select
	var $selectbox = $('.input-select, select');
	if ($selectbox.length > 0) {
        $selectbox.each(function() {
			var $this = $(this);
            $this.select2();
		});
	}
    
    
    // Function For Toggle Class On click
    function tglcls(tigger,action,connect,connect2){
        var $toglr = tigger, $tgld = action,$tgcon = connect,$tgcon2 = connect2;
        $toglr.on("click",function(){
            $tgld.toggleClass('active');
            $toglr.toggleClass('active');
            if($tgcon.hasClass('active')){
                $tgcon.removeClass('active');
            }
            if($tgcon2.hasClass('active')){
                $tgcon2.removeClass('active');
            }
            return false;
        });
    }
    var $toggle_action = $('.toggle-action'), 
        $topbar_action = $('.topbar-action'), 
        $toggle_nav = $('.toggle-nav'), 
        $sidebar = $('.user-sidebar'),
        $sidebar_overlay = $('.user-sidebar-overlay');
	if ($toggle_action.length > 0 ) {
		tglcls($toggle_action,$topbar_action,$sidebar,$toggle_nav);
	}
	if ($toggle_nav.length > 0 ) {
		tglcls($toggle_nav,$sidebar,$topbar_action,$toggle_action);
	}
    if ($sidebar_overlay.length > 0 ) {
		$sidebar_overlay.on("click",function(){
            $sidebar.removeClass('active');
            $toggle_nav.removeClass('active');
        });
	}
    if(wwCurrent < 991){
        $sidebar.delay(500).addClass('user-sidebar-mobile');
    }else{
        $sidebar.delay(500).removeClass('user-sidebar-mobile');
    }
    $win.on('resize', function () { 
        if(wwCurrent < 991){
            $sidebar.delay(500).addClass('user-sidebar-mobile');
        }else{
            $sidebar.delay(500).removeClass('user-sidebar-mobile');
        }
	});



    
    // Tooltip
    var $tooltip = $('[data-toggle="tooltip"]');
    if($tooltip.length > 0){
        $tooltip.tooltip();
    }
    

    
    // Toggle
    function toggleTab(triger,action,connect){
        triger.on('click',function(){
            action.addClass('active');
            if(connect.hasClass('active')){
                connect.removeClass('active');
            }
            return false;
        });
    }
    

    
    // Dropzone




    /*-- @v101-e */
})(jQuery);