import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import ParkrunMap from './components/ParkrunMap';
import About from './components/About';
import withTracker from './withTracker';

export default () => (
  <Layout>
    <Route exact path='/' component={withTracker(ParkrunMap)} />
    <Route exact path='/wheelchair-friendly' component={withTracker(ParkrunMap)} />
    <Route exact path='/buggy-friendly' component={withTracker(ParkrunMap)} />
    <Route exact path='/about' component={withTracker(About)} />
  </Layout>
);
