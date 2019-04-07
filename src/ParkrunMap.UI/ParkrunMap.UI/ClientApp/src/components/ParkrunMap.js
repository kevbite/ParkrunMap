import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import Map from './Map'
import { actionCreators as locationActionCreators } from '../store/Location';
import { actionCreators as parkrunsActionCreators } from '../store/Parkruns';

class ParkrunMap extends Component {
  componentWillMount() {
    this.props.locationActions.requestLocation();
  }

  onBoundsChanged = ({ bottomLeft, topRight }) => {
    this.props.parkrunsActions.requestParkruns(bottomLeft.lat, bottomLeft.lon, topRight.lat, topRight.lon);
  }

  render() {

    return (
      <Map
        onBoundsChanged={this.onBoundsChanged}
        userLocation={this.props.location}
        parkruns={this.props.parkruns}
      />
    );
  }
}

function mapDispatchToProps(dispatch) {
  return {
    locationActions: bindActionCreators(locationActionCreators, dispatch),
    parkrunsActions: bindActionCreators(parkrunsActionCreators, dispatch)
  }
}

function mapStateToProps(state) {

  const props = {
    location: state.location.location,
    parkruns: state.parkruns.parkruns
  };

  if (state.routing.location.pathname !== '/') {

    var parkruns = state.parkruns.parkruns.filter(parkrun => {
      const filters = {
        wheelchairFriendly: state.routing.location.pathname === '/wheelchair-friendly',
        buggyFriendly: state.routing.location.pathname === '/buggy-friendly'
      };
      var selected = false;
      if (filters.wheelchairFriendly && parkrun.features.wheelchairFriendly) {
        selected = true;
      }
      if (filters.buggyFriendly && parkrun.features.buggyFriendly) {
        selected = true;
      }
      
      return selected;
    });

    props.parkruns = parkruns;
  }

  return props;
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ParkrunMap);
