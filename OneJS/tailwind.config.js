const { theme, paths, plugins: onejsPlugins, corePlugins } = require('./ScriptLib/onejs-tw-config')
const plugin = require("tailwindcss/plugin")

module.exports = {
    content: [...paths, "./index.js"],
    theme: theme,
    plugins: [...onejsPlugins, plugin(function ({ addUtilities }) {
        addUtilities({
            ".default-bg-color": { "background-color": "white" },
            ".accented-bg-color": { "background-color": "#fde047" },
            ".hover-bg-color": { "background-color": "rgb(0 0 0 / 0.1)" },
            ".default-text-color": { "color": "#4b5563" },
            ".active-text-color": { "color": "#cd8c06" },
            ".highlighted-text-color": { "color": "#854d0e" },
        })
    })],
    corePlugins: corePlugins
}