import React from 'react';
import { connect } from 'react-redux';
import { Helmet } from "react-helmet";

const About = props => (
  <div>
    <Helmet>
      <title>About the map</title>
      <meta name="description" content="Find out information about the The Unofficial Parkrun Event Map" />}
    </Helmet>
    <h1>Hello, and welcome to the unofficial parkrun event map!</h1>
    <p>Here's some useful links</p>
    <ul>
      <li><a href='http://www.parkrun.org.uk'>www.parkrun.org.uk</a></li>
      <li><a href='https://github.com/kevbite/ParkrunMap'>GitHub page</a> where all the code lives</li>
    </ul>
  </div>
);

export default connect()(About);
