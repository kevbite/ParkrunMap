import React from 'react';
import {
  Nav,
  Navbar,
  NavItem,
  NavLink,
  NavbarBrand,
  NavbarToggler,
  Collapse
} from 'reactstrap';
import './NavMenu.css';

export default class NavMenu extends React.Component {
  
  state = {
    isOpen: false
  };

  toggle = () => {
    this.setState({
      isOpen: !this.state.isOpen
    });
  }

  render() {
    return (
      <div>
        <Navbar color="dark" dark expand="sm">
          <NavbarBrand href="/">
            <img src="/favicon.ico" width="32" height="32" class="d-inline-block align-top" alt="Parkrun event map" />
            Parkrun Map
          </NavbarBrand>
          <NavbarToggler onClick={this.toggle} />
          <Collapse isOpen={this.state.isOpen} navbar>
            <Nav className="ml-auto" navbar>
              <NavItem>
                <NavLink href="/">Map</NavLink>
              </NavItem>
              <NavItem>
                <NavLink href="/about">About</NavLink>
              </NavItem>
              <NavItem>
                <NavLink href="https://github.com/kevbite/ParkrunMap">GitHub</NavLink>
              </NavItem>
            </Nav>
          </Collapse>
        </Navbar>
      </div>
    );
  }
}