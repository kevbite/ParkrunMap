import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import ParkrunMap from './components/ParkrunMap';
import About from './components/About';
import withTracker from './withTracker';
import {Helmet} from "react-helmet";

const title = "Unofficial Parkrun Event Map";
const description = "The Unofficial Parkrun Event Map that helps you find parkruns.";

export default () => (
  <Layout>
    <Helmet
      defaultTitle={title}
      titleTemplate={`%s - ${title}`}>
        <meta name="description" content={description} />
        <meta name="keywords" content="Parkrun,Running,Events,Map,Cancelled" />

        <meta property="og:title" content={title} />
        <meta property="og:description" content={description} />
        <meta property="og:image" content="https://parkrun-map.com/images/og-image.png" />
        <meta property="og:url" content="https://parkrun-map.com/" />

        <meta name="twitter:card" content="summary" />
        <meta name="twitter:site" content="@ParkrunMap" />
        <meta name="twitter:title" content={title} />
        <meta name="twitter:description" content={description} />
        <meta name="twitter:image" content="https://parkrun-map.com/images/twitter-card-image.png" />
    </Helmet>
    
    <Route exact path='/' component={withTracker(ParkrunMap)} />
    <Route exact path='/wheelchair-friendly' component={withTracker(ParkrunMap)} />
    <Route exact path='/buggy-friendly' component={withTracker(ParkrunMap)} />
    <Route exact path='/about' component={withTracker(About)} />
  </Layout>
);
