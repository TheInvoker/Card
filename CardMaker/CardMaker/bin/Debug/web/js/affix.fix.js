 $(window).on('scroll resize',function() {
   var $affixElement = $('div[data-spy="affix"]');
    $affixElement.width($affixElement.parent().width());
});