const requestLocationType = 'REQUEST_LOCATION';
const receiveLocationType = 'RECEIVE_LOCATION';
const initialState = {
  location: { latitude: 51.509865, longitude: -0.118092 },
  isLoading: false,
  isLoaded: false
};

export const actionCreators = {
  requestLocation: () => (dispatch) => {    

    dispatch({ type: requestLocationType });

    if (navigator && navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        position => {
          const location = { latitude: position.coords.latitude, longitude: position.coords.longitude };

          dispatch({ type: receiveLocationType, location });
        }
      );
    }
  }
};

export const reducer = (state, action) => {
  state = state || initialState;

  if (action.type === requestLocationType) {
    return {
      ...state,
      isLoading: true,
      isLoaded: false
    };
  }

  if (action.type === receiveLocationType) {
    return {
      ...state,
      location: action.location,
      isLoading: false,
      isLoaded: true
    };
  }

  return state;
};
