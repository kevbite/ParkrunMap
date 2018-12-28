import React from 'react';
import { Route } from 'react-router';
import Layout from './components/Layout';
import ParkrunMap from './components/ParkrunMap';
import About from './components/About';

export default () => (
  <Layout>
    <Route exact path='/' component={ParkrunMap} />
    <Route exact path='/about' component={About} />
  </Layout>
);
