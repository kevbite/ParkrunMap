import React from 'react';
import {
  Nav,
  Navbar,
  NavItem,
  NavLink,
  NavbarBrand,
  NavbarToggler,
  Collapse,
  Container
} from 'reactstrap';
import './NavMenu.css';
import NavSpinner from './NavSpinner';

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
          <Container>
            <NavbarBrand href="/">
              <img src="/favicon.ico" width="32" height="32" className="d-inline-block align-top" alt="Parkrun event map" />
              Parkrun Map
            </NavbarBrand>
            <NavSpinner />
            <NavbarToggler onClick={this.toggle} />
          </Container>
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