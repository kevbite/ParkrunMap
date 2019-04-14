import React, { Component } from 'react';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import Map from './Map'
import { actionCreators as locationActionCreators } from '../store/Location';
import { actionCreators as parkrunsActionCreators } from '../store/Parkruns';
import { Helmet } from 'react-helmet';
import parkrunSelector from '../selectors/parkrunSelector';

class ParkrunMap extends Component {
  componentWillMount() {
    this.props.locationActions.requestLocation();
  }

  onBoundsChanged = ({ bottomLeft, topRight }) => {
    this.props.parkrunsActions.requestParkruns(bottomLeft.lat, bottomLeft.lon, topRight.lat, topRight.lon);
  }

  render() {
    return (
      <div>
        <Helmet>
          {this.props.title && <title>{this.props.title}</title>}
          {this.props.description && <meta name="description" content={this.props.description} />}
          {this.props.keywords && <meta name="keywords" content={this.props.keywords} />}
        </Helmet>
        <Map
          onBoundsChanged={this.onBoundsChanged}
          userLocation={this.props.location}
          parkruns={this.props.parkruns}
        />
      </div>
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
    keywords: "Parkrun,Running,Events,Map,Cancelled",
    location: state.location.location
  };

  const filters = {
    wheelchairFriendly: state.routing.location.pathname === '/wheelchair-friendly',
    buggyFriendly: state.routing.location.pathname === '/buggy-friendly'
  };

  props.parkruns = parkrunSelector(state.parkruns.parkruns, filters);


  if (filters.wheelchairFriendly) {
    props.title = "Wheelchair friendly parkruns";
    props.description = "Find wheelchair friendly parkruns";
    props.keywords += ",wheelchair,disabled,accessibility";
  } else if (filters.buggyFriendly) {
    props.title = "Buggy friendly parkruns";
    props.description = "Find buggy friendly parkruns";
    props.keywords += ",buggy,baby,prams,pushchairs";
  }
  
  return props;
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ParkrunMap);
