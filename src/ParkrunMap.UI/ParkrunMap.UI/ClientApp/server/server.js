import path from 'path';
import express from 'express';
import fs from 'fs-extra';
import App from '../src/App';
import { renderToString } from 'react-dom/server'
import React from 'react'
import { ConnectedRouter } from 'react-router-redux';
import configureStore from '../src/store/configureStore';
import { Provider } from 'react-redux';
import createHistory from 'history/createMemoryHistory';

const port = process.env.PORT || 3000;
const app = express();

global.navigator = {
    userAgent: 'node.js'
};

app.use(express.static(path.resolve(__dirname, '../build')));

app.get(['/', '/wheelchair-friendly', '/buggy-friendly', '/about'], function (req, res) {
    let index = fs.readFileSync('./build/index.html', "utf-8");

    const history = createHistory({
        initialEntries: [req.path],
    });

    const initialState = {};

    const store = configureStore(history, initialState);

    const appRendered = renderToString(
        <Provider store={store}>
            <ConnectedRouter history={history}>
                <App />
            </ConnectedRouter>
        </Provider>
    );

    index = index.replace(`<div id="root"></div>`, `<div id="root">${appRendered}</div>`)

    res.send(index);
});

app.listen(port, '0.0.0.0', () => console.info(`Listening at http://localhost:${port}`));