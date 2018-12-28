import React from 'react';
import NavMenu from './NavMenu';

export default props => (
  <div>
    <NavMenu />

    {props.children}
  </div>
);
