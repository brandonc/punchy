$(document).ready(function() {

    var isAjaxing = false;

    $('.close').click(function() {
        $this = $(this);
        $($this.attr('rel')).slideToggle("fast", function() {
            if ($(this).is(':visible'))
                $this.text($this.text().replace(/Show/, 'Hide'));
            else
                $this.text($this.text().replace(/Hide/, 'Show'));
        });
        return false;
    });

    $('a[href="#close"]').live('click', function() {
        $(this).parents('.popover').remove();
    });

    // Show login dialog
    $('a[href$="/account/login"]').click(function() {
        $(this).buzzard({ dataUrl: $(this).attr('href'), dialogClass: 'login-dialog' });

        return false;
    });

    $('a[href=#ask-question]').filter('a[rel]').click(function() {
        $(this).buzzard({ dataUrl: $(this).attr('rel'), dialogClass: 'ask-question-dialog' });

        return false;
    });

    $('a.nugget-post-response')

    // Block the user - checked
    $('a[href="#block-user"]').filter('a[rel]').click(function() {
        var link = this;
        if (!isAjaxing) {
            isAjaxing = true;
            $.ajax({
                type: 'GET',
                url: $(this).attr("rel"),
                success: function(data) {
                    blockingcomplete(link, "a[href=\"#unblock-user\"]", data);
                    isAjaxing = false;
                },
                failure: function(data) {
                    isAjaxing = false;
                }
            });
        }

    });

    // UnBlock the user - checked
    $('a[href="#unblock-user"]').filter('a[rel]').click(function() {
        var link = this;
        if (!isAjaxing) {
            isAjaxing = true;
            $.ajax({
                type: 'GET',
                url: $(this).attr("rel"),
                success: function(data) {
                    blockingcomplete(link, "a[href=\"#block-user\"]", data);
                    isAjaxing = false;
                },
                failure: function(data) {
                    isAjaxing = false;
                }
            });
        }
    });

    function blockingcomplete(link, selector, data) {
        $(link).hide(function() {
            $(link).siblings(selector).show()
        });

        var msg = $("#user-action-message");
        if (!data.Success) {
            $(msg).addClass("failure");
            $(msg).text(data.ErrorMessage);
            $(msg).show();
        }
    }


    var isSharing = false;

    // Share nugget
    $('a.share').live('click', function() {
        var href = $(this).attr('href');
        var $this = $(this);

        function moveshare() {
            if ($this.parents('#featured-nugget').size() > 0) {
                $('.sharechoice').css('left', $this.offset().left - $('.sharechoice').width() - 3)
                    .css('top', parseInt($this.offset().top));
            } else {
                $('.sharechoice').css('left', $this.offset().left + 17)
                    .css('top', parseInt($this.offset().top));
            }
        }

        if (!isSharing && $('.sharechoice').length <= 0) {
            isSharing = true;

            $.ajax({
                url: href,
                dataType: 'html',
                success: function(data) {
                    $(document.body).append(data);

                    moveshare();

                    $(window).bind('resize', moveshare);

                    $('.sharechoice .closepopover').click(function() {
                        $(window).unbind('resize', moveshare);
                    });

                    isSharing = false;
                },
                type: 'GET'
            });
        }

        return false;
    });


    // Voting
    $('#nugget-list .up-button').live('click', function() {
        if (!isAjaxing) {
            isAjaxing = true;
            var nugget = $(this).parents('.nugget');
            vote(this, nugget, $(this).attr('href') + '&isfeatured=False&isdetail=False');
        }
        return false;
    });

    $('#nugget-list .down-button').live('click', function() {
        if (!isAjaxing) {
            isAjaxing = true;

            var nugget = $(this).parents('.nugget');
            vote(this, nugget, $(this).attr('href') + '&isfeatured=False&isdetail=False');
        }
        return false;
    });

    $('#featured-nugget .up-button, .item-detail .up-button').live('click', function() {
        if (!isAjaxing) {
            isAjaxing = true;

            var nugget = $(this).parents('.nugget');
            vote(this, nugget, $(this).attr('href') + '&isfeatured=True&isdetail=False');

            var skipquestion = nugget.find('.nugget-skipquestion');
            if (skipquestion.size() > 0)
                get_next_featured_question(skipquestion.attr('href'), nugget);
        }
        return false;
    });


    $('#featured-nugget .down-button, .item-detail .down-button').live('click', function() {
        if (!isAjaxing) {
            isAjaxing = true;

            var nugget = $(this).parents('.nugget');
            vote(this, nugget, $(this).attr('href') + '&isfeatured=True&isdetail=False');

            var skipquestion = nugget.find('.nugget-skipquestion');
            if (skipquestion.size() > 0)
                get_next_featured_question(skipquestion.attr('href'), nugget);
        }
        return false;
    });

    // Skip nugget
    $('a.nugget-skipquestion').live('click', function() {
        if (!isAjaxing) {
            isAjaxing = true;
            var nugget = $(this).parents('.nugget');

            get_next_featured_question($(this).attr('href'), nugget);
        }
        return false;
    });

    function get_next_featured_question(href, nugget) {
        $.ajax({
            type: 'POST',
            url: href,
            success: function(data) {
                nugget.parents('#featured-wrapper').siblings('.divider:first').remove();
                nugget.parents('#featured-wrapper').replaceWith(data);

                isAjaxing = false;
            },
            failure: function(data) {
                isAjaxing = false;
            },
            dataType: 'html'
        });
    }

    // Voting ajax method - checked
    function vote(obj, nugget, url) {

        $.ajax({
            type: 'POST',
            async: false,
            url: url,
            success: function(data) {
                if (nugget.parents('.item-detail').size())
                    window.location.reload(false);
                else if (nugget.parents('#featured-wrapper').size() == 0)
                    nugget.replaceWith(data);

                isAjaxing = false;
            },
            failure: function(data) {
                isAjaxing = false;
            },
            dataType: 'html'
        });

    }

    // Simulate button click for login if enter is pressed
    $('#login-form input').live('keypress', function(e) {
        if (e.which == 13) {
            $(this).parent('form').find('a.btnsignin').click();
            return false;
        } else {
            return true;
        }
    });

    var isPosting = false;
    // Submit login form, redirect if successful
    $('a.btnsignin').live('click', function() {
        if (!isPosting) {
            isPosting = true;
            $.post(
                $(this).parent('form').attr('action'),
                $('#login-form').serialize(),
                function(data) {
                    if (data.Success) {
                        window.location.reload(false);
                    } else {
                        $('.login-dialog').find('.errors').html(data.ErrorMessage);
                    }

                    isPosting = false;
                },
                'json'
            );
        }
        return false;
    });

    // Generic form submit using anchor tag instead of input:submit
    $('a[href="#submit"]').live('click', function() {
        return $(this).parent('form').submit();
    });

    // Open/collapse responses
    $('a[href="#responses"],a[href="#postresponse"]').live('click',
        function() {
            var link = $(this).siblings('a[href="#responses"]');
            if (link.size() == 0)
                link = $(this);

            var responses = $(this).parents('.nugget').find('.responses');

            responses.toggle("slow", function() {
                if ($(this).is(':visible')) {
                    link.text("Close Responses");
                    $(this).find('textarea').focus();
                }
                else {
                    link.text("Responses");
                }
            });
        }
    );

    $('.action .action-submit').live("click",
        function() {
            PostQuestion($(this).attr('link'), $(this).attr('rel'));
        }
    );

    function PostQuestion(url, categorySlug) {
        var question = $('#questionBody');
        var tags = $('#questionTags');

        var post = { Body: question.val(), TagsCommaSeparated: tags.val(), CategorySlug: categorySlug };

        if (!isPosting) {
            isPosting = true;
            showLoading();

            $.ajax({
                type: "POST",
                //dataType: "json",
                data: post,
                url: url,
                success: function(msg) {
                    try {
                        if (msg != null && msg.NuggetDetailsLink != undefined) {
                            document.location = msg.NuggetDetailsLink;
                        }
                        else {
                            $('#ask-question-field').parent().html(msg);

                            hideLoading();
                            isPosting = false;
                        }
                    }
                    catch (err) {
                        $('#ask-question-field').parent().html(err);

                        hideLoading();
                        isPosting = false;
                    }
                },
                failure: function(data) {
                    isAjaxing = false;
                }
            });
        }
    }

    function showLoading() {
        if ($("#ajax-loading").length <= 0)
            $('body').append("<div id=\"ajax-loading\" class=\"loading\"></div>");
    }

    function hideLoading() {
        $("#ajax-loading").remove();
    }
});
