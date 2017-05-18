module.exports = {
    plugins: [
        require("postcss-import"),
        require('postcss-cssnext')({
            browsers: ['last 2 version'],
            features: {
                customProperties: {
                    variables: {
                        '--color-white': '#fff',
                        '--color-red': '#dd0a2d',
                        '--color-dark-red': '#dd0a2d',
                        '--color-black': '#000',
                        '--color-light-gray': '#ccc',
                        '--color-light-gray-2': '#eee',
                        '--color-light-gray-3': '#c7c7c7',
                        '--color-light-gray-4': '#aaa',
                        '--text-color-light': '#8f8f8f',
                        '--text-color-dark': '#333',
                        '--header-bg': '#373737',
                        '--font-custom': '"Open Sans", Arial, Helvetica, sans-serif',
                        '--font-general': '"Open Sans", Arial, Helvetica, sans-serif'
                    }
                },
                customMedia: {
                    extensions: {
                        '--for-phone-only': ' (width <= 599px)',
                        '--for-tablet-portrait-up': ' (width >= 600px)',
                        '--for-tablet-portrait-down': ' (width < 900px)',
                        '--for-tablet-landscape-up': ' (width >= 900px)',
                        '--for-desktop-up': '(width >= 1200px)',
                        '--for-big-desktop-up': '(width >= 1800px)'
                    }
                }
            }
        }),
        require('precss')
    ]
};
