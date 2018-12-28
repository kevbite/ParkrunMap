import React from 'react';
import { connect } from 'react-redux';

const About = props => (
  <div>
    <h1>Hello, and welcome to the unofficial parkrun event map!</h1>
    <p>Here's some useful links</p>
    <ul>
      <li><a href='http://www.parkrun.org.uk'>www.parkrun.org.uk</a></li>
      <li><a href='https://github.com/kevbite/ParkrunMap'>GitHub page</a> where all the code lives</li>
    </ul>
   </div>
);

export default connect()(About);
