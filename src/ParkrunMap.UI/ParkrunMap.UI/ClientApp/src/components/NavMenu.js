import React from 'react';
import {
  Nav,
  Navbar,
  NavItem,
  NavbarBrand,
  NavbarToggler,
  NavLink,
  Collapse,
  Container
} from 'reactstrap';
import './NavMenu.css';
import NavSpinner from './NavSpinner';
import PropTypes from 'prop-types';
import { Link } from "react-router-dom";

class NavMenu extends React.Component {

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
            <Nav navbar>
              <NavItem>
                <NavLink
                  tag={Link}
                  active={this.context.router.route.location.pathname === '/'}
                  to="/">
                  All Parkruns
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink
                  tag={Link}
                  active={this.context.router.route.location.pathname === '/wheelchair-friendly'}
                  to="/wheelchair-friendly">
                  Wheelchair Friendly
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink
                  tag={Link}
                  active={this.context.router.route.location.pathname === '/buggy-friendly'}
                  to="/buggy-friendly">
                  Buggy Friendly
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink
                  tag={Link}
                  active={this.context.router.route.location.pathname === '/about'}
                  to="/about">
                  About
                </NavLink>
              </NavItem>
            </Nav>
          </Collapse>
        </Navbar>
      </div>
    );
  }
}


NavMenu.contextTypes = {
  router: PropTypes.object
};

export default NavMenu;
