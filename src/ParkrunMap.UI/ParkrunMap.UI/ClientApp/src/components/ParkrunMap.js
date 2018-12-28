import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import Map from './Map'
import { actionCreators as locationActionCreators } from '../store/Location';
import { actionCreators as parkrunsActionCreators } from '../store/Parkruns';

class ParkrunMap extends Component {
  componentWillMount() {
    this.props.locationActions.requestLocation();
    this.props.parkrunsActions.requestParkruns();
  }

  render() {
    return (
      <Map
        defaultCenter={this.props.location}
        parkruns={this.props.parkruns}
        onParkrunMarkerClick={() => alert('Hello')}
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
  return {
    location: state.location.location,
    parkruns: state.parkruns.parkruns
  }
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ParkrunMap);
