window.themeSwitcher = {
    setFont: function (font) {
        document.documentElement.style.setProperty("--font-base-family", font);
    },
    setFontSize: function (size) {
        document.documentElement.style.setProperty("--font-base", size);
    },
    loadGoogleFont: function (fontName) {
        const id = "dynamic-font-link-" + fontName;
        if (!document.getElementById(id)) {
            const link = document.createElement("link");
            link.id = id;
            link.rel = "stylesheet";
            link.href = `https://fonts.googleapis.com/css2?family=${fontName.replace(/ /g, '+')}&display=swap`;
            document.head.appendChild(link);
        }
        document.body.style.fontFamily = `'${fontName}', sans-serif`;
    }
};
