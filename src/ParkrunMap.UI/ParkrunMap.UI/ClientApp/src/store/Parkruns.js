const requestParkrunsType = 'REQUEST_PARKRUNS';
const receiveParkrunsType = 'RECEIVE_PARKRUNS';
const initialState = { parkruns: [], isLoading: false };

export const actionCreators = {
  requestParkruns: () => async (dispatch) => {    

    dispatch({ type: requestParkrunsType });

    const url = 'https://parkrun-map.azurewebsites.net/api/parkruns/uk';
    const response = await fetch(url);
    const parkruns = await response.json();

    dispatch({ type: receiveParkrunsType, parkruns });
  }
};

export const reducer = (state, action) => {
  state = state || initialState;

  if (action.type === requestParkrunsType) {
    return {
      ...state,
      isLoading: true
    };
  }

  if (action.type === receiveParkrunsType) {
    return {
      ...state,
      parkruns: action.parkruns,
      isLoading: false
    };
  }

  return state;
};
