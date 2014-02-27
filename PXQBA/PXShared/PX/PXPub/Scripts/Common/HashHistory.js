// HashHistory:
// Description: Wrapper for the hasher and crossroads plugins. Listens for the PX_SET_HASH event and then triggers hash update which is handle
// by a crossover parse function
var HashHistory = function ($) {
    var _static = {
        initialized: false,
        defaultHash: 'index',
        fn: {
            onHasherInit: function (curHash) {
                if (curHash === '') {
                    hasher.replaceHash(_static.defaultHash);
                } else {
                    _static.fn.parseHash(curHash);
                }
            },
            parseHash: function (newHash, oldHash) {
                crossroads.parse(newHash);
            }
        }
    };

    return {
        IsInit: function() {
            return _static.initialized;
        },
        Init: function (defaultHash) {
            if (_static.initialized)
                return; // already inited

            _static.defaultHash = defaultHash || _static.defaultHash;

            //Only set these handlers.
            hasher.changed.add(_static.fn.parseHash);
            hasher.initialized.add(_static.fn.onHasherInit);

            //Listens to the PX_SET_HASH event to set the hash
            $(PxPage.switchboard).bind('PX_SET_HASH', function (event, newHash, replace) {
                if (newHash !== undefined) {
                    if (replace) {
                        //hasher.replaceHash(newHash);
                        HashHistory.ReplaceHashWithPossibleRedirect(newHash);
                    } else {
                        hasher.setHash(newHash);
                    }
                }
            });

            hasher.init();
            _static.initialized = true;
            $(PxPage.switchboard).trigger("hashinitialized");

        },
        InitializeWithArgs: function (args) {
            if (_static.initialized)
                return; // already inited

            _static.defaultHash = args.defaultHash || _static.defaultHash;

            if(args.onHasherChanged)
                hasher.changed.add(args.onHasherChanged);
            if (args.onHasherInit)
                hasher.initialized.add(args.onHasherInit);

            hasher.init();
            _static.initialized = true;
            $(PxPage.switchboard).trigger("hashinitialized");

        },
        SetHash: function (newHash) {
            hasher.setHash(newHash);
        },
        SetHashSilently: function (newHash) {
            hasher.changed.active = false;
            hasher.setHash(newHash);
            hasher.changed.active = true;
        },
        ReplaceHashWithPossibleRedirect: function (hash)
        {
            var old_hash = window.location.hash.replace(/^#/, '');
            hasher.changed.active = false;
            hasher.replaceHash(hash);
            hasher.changed.active = true;
            hasher.changed.dispatch(hash, old_hash);
        },
        SetHashWithPossibleRedirect: function (hash)
        {
            var old_hash = window.location.hash.replace(/^#/, '');
            hasher.changed.active = false;
            hasher.setHash(hash);
            hasher.changed.active = true;
            hasher.changed.dispatch(hash, old_hash);
        }
    };
} (jQuery);